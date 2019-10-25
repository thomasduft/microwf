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
  public class WorkflowEngineService : IWorkflowEngineService
  {
    private readonly IWorkflowRepository _repository;
    private readonly ILogger<WorkflowEngineService> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IUserContextService _userContext;

    public WorkflowEngineService(
      IWorkflowRepository repository,
      ILogger<WorkflowEngineService> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserContextService userContext
    )
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
      return this._repository.Find(id, type);
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

      using (var transaction = _repository.Database.BeginTransaction())
      {
        try
        {
          Workflow workflow = null;
          var execution = GetExecution(param.Instance.Type);
          var entity = param.Instance as IWorkflowInstanceEntity;

          await _repository.ApplyChangesAsync(); // so entity id gets resolved!

          workflow = await this.FindOrCreate(
            entity.Id,
            param.Instance.Type,
            param.Instance.State
          );

          EnsureWorkflowVariables(workflow, param);

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            await PersistWorkflow(workflow, param);
            if (result.HasAutoTrigger)
            {
              this._repository.AddWorkItemEntry(result.AutoTrigger, entity);
            }

            await _repository.ApplyChangesAsync();

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

    private async Task<Workflow> FindOrCreate(int id, string type, string state, string assignee = null)
    {
      var list = await _repository.ListAsync(new GetWorkflowInstanceVariables(type, id));
      var workflow = list.FirstOrDefault();
      if (workflow == null)
      {
        workflow = Workflow.Create(id, type, state, assignee);
        await _repository.AddAsync(workflow);
      }

      return workflow;
    }

    private void EnsureWorkflowVariables(Workflow workflow, TriggerParam param)
    {
      if (workflow.WorkflowVariables.Count == 0) return;

      foreach (var workflowVariable in workflow.WorkflowVariables)
      {
        var variable = WorkflowVariable.ConvertContent(workflowVariable);
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

    private async Task<bool> WorkflowIsCompleted(TriggerParam triggerParam)
    {
      var triggerResults
        = await GetTriggersAsync(triggerParam.Instance, triggerParam.Variables);

      return triggerResults.All(r => !r.CanTrigger);
    }
  }
}