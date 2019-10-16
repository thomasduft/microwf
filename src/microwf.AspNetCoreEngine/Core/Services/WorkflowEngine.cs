using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public class WorkflowEngine<TContext> : IWorkflowEngine where TContext : EngineDbContext
  {
    private readonly EngineDbContext _context;
    private readonly ILogger<WorkflowEngine<TContext>> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IUserContextService _userContext;

    public WorkflowEngine(
      TContext context,
      ILogger<WorkflowEngine<TContext>> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserContextService userContext
    )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger;
      _workflowDefinitionProvider = workflowDefinitionProvider;
      _userContext = userContext;
    }

    public async Task<TriggerResult> CanTriggerAsync(TriggerParam param)
    {
      if (param == null) throw new ArgumentNullException(nameof(param));

      var execution = GetExecution(param.Instance.Type);

      return await Task.FromResult(execution.CanTrigger(param));
    }

    public async Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    )
    {
      if (instance == null) throw new ArgumentNullException(nameof(instance));

      var execution = GetExecution(instance.Type);

      return await Task.FromResult(execution.GetTriggers(instance, variables));
    }

    public async Task<TriggerResult> TriggerAsync(TriggerParam param)
    {
      if (param == null) throw new ArgumentNullException(nameof(param));

      _logger.LogTrace(
        "Trigger instance: {Instance}",
        LogHelper.SerializeObject(param.Instance)
       );

      var result = param.Instance is IWorkflowInstanceEntity
        ? await this.TriggerForPersistingInstance(param)
        : this.TriggerForNonPersistingInstance(param);

      if (result.IsAborted)
      {
        _logger.LogInformation(
          "Trigger instance: {Instance} aborted! Aborting reasons: {Reasons}",
          LogHelper.SerializeObject(param.Instance),
          LogHelper.SerializeObject(result.Errors)
        );
      }

      return result;
    }

    public IWorkflow Find(int id, Type type)
    {
      return (IWorkflow)_context.Find(type, id);
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }

    private TriggerResult TriggerForNonPersistingInstance(TriggerParam param)
    {
      var execution = GetExecution(param.Instance.Type);

      return execution.Trigger(param);
    }

    private async Task<TriggerResult> TriggerForPersistingInstance(TriggerParam param)
    {
      TriggerResult result;

      var entity = param.Instance as IWorkflowInstanceEntity;
      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {
          Workflow workflow = null;
          var execution = GetExecution(param.Instance.Type);

          await _context.SaveChangesAsync(); // so entity id gets resolved!

          workflow = this.FindOrCreate(
            entity.Id,
            param.Instance.Type,
            param.Instance.State
          );

          EnsureWorkflowVariables(workflow, param);

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            await PersistWorkflow(workflow, param);
            if (result.HasAutoTrigger) CreateWorkItemEntry(result.AutoTrigger, entity);

            await _context.SaveChangesAsync();

            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();

          _logger.LogError(
            "Trigger with TriggerParameter: {TriggerParameter} failed! {Exception}",
            LogHelper.SerializeObject(param),
            ex
          );

          var transitionContext = new TransitionContext(param.Instance);
          transitionContext.AddError(ex.ToString());

          result = new TriggerResult(
            param.TriggerName,
            transitionContext,
            false
          );
        }
      }

      return result;
    }

    private Workflow FindOrCreate(int id, string type, string state, string assignee = null)
    {
      var workflow = _context.Workflows
        .Include(v => v.WorkflowVariables)
        .SingleOrDefault(w => w.CorrelationId == id && w.Type == type);
      if (workflow == null)
      {
        workflow = Workflow.Create(id, type, state, assignee);
        _context.Workflows.Add(workflow);
      }

      return workflow;
    }

    private void EnsureWorkflowVariables(Workflow workflow, TriggerParam param)
    {
      if (workflow.WorkflowVariables.Count == 0) return;

      foreach (var workflowVariable in workflow.WorkflowVariables)
      {
        var variable = JsonConvert.DeserializeObject(
          workflowVariable.Content,
          KeyBuilder.FromKey(workflowVariable.Type)
        );
        if (variable is WorkflowVariableBase)
        {
          var key = workflowVariable.Type;
          if (param.Variables.ContainsKey(key))
          {
            param.Variables[key] = variable as WorkflowVariableBase;
          }
          else
          {
            param.Variables.Add(key, variable as WorkflowVariableBase);
          }
        }
      }
    }

    private async Task PersistWorkflow(Workflow workflow, TriggerParam param)
    {
      if (workflow == null) throw new ArgumentNullException(nameof(workflow));

      // persisting workflow variables
      if (param.HasVariables)
      {
        foreach (var v in param.Variables)
        {
          var variable = workflow.WorkflowVariables
            .FirstOrDefault(variables => variables.Type == v.Key);

          if (variable != null)
          {
            variable.Content = JsonConvert.SerializeObject(v.Value);
          }
          else
          {
            workflow.AddVariable(v.Value);
          }
        }
      }

      // keeping workflow entity nsync
      var entity = param.Instance as IWorkflowInstanceEntity;
      var assignableEntity = param.Instance as IAssignableWorkflow;
      if (entity != null)
      {
        workflow.Type = entity.Type;
        workflow.Assignee = assignableEntity.Assignee;

        workflow.AddHistoryItem(workflow.State, entity.State, _userContext.UserName);
        workflow.State = entity.State;
      }

      if (await this.WorkflowIsCompleted(param))
      {
        workflow.Completed = SystemTime.Now();
      }
    }

    private void CreateWorkItemEntry(AutoTrigger autoTrigger, IWorkflowInstanceEntity entity)
    {
      var workItem = WorkItem.Create(
        autoTrigger.Trigger,
        entity.Id,
        entity.Type,
        autoTrigger.DueDate
      );

      this._context.WorkItems.Add(workItem);
    }

    private async Task<bool> WorkflowIsCompleted(TriggerParam triggerParam)
    {
      var triggerResults
        = await GetTriggersAsync(triggerParam.Instance, triggerParam.Variables);

      return triggerResults.All(r => !r.CanTrigger);
    }
  }
}