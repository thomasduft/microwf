using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using WebApi.Common;
using WebApi.Domain;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowEngine
  {
    Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    );

    Task<TriggerResult> CanTriggerAsync(TriggerParam param);

    Task<TriggerResult> TriggerAsync(TriggerParam param);

    IWorkflow Find(int id, Type type);
  }

  public class WorkflowEngine<TContext> : IWorkflowEngine where TContext : EngineDbContext
  {
    private readonly EngineDbContext _context;
    private readonly ILogger<WorkflowEngine<TContext>> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;

    public WorkflowEngine(
      TContext context,
      ILogger<WorkflowEngine<TContext>> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider
    )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger;
      _workflowDefinitionProvider = workflowDefinitionProvider;
    }

    public async Task<TriggerResult> CanTriggerAsync(TriggerParam param)
    {
      if (param == null) throw new InvalidOperationException(nameof(param));

      var execution = GetExecution(param.Instance.Type);

      return await Task.FromResult(execution.CanTrigger(param));
    }

    public async Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    )
    {
      if (instance == null) throw new InvalidOperationException(nameof(instance));

      var execution = GetExecution(instance.Type);

      return await Task.FromResult(execution.GetTriggers(instance, variables));
    }

    public async Task<TriggerResult> TriggerAsync(TriggerParam param)
    {
      if (param == null) throw new InvalidOperationException(nameof(param));

      var entity = param.Instance as IEntityWorkflow;
      if (entity == null) throw new Exception("No entity given!");

      TriggerResult result = null;
      using (var transaction = this._context.Database.BeginTransaction())
      {
        try
        {
          Workflow workflow = null;

          var execution = GetExecution(param.Instance.Type);

          await _context.SaveChangesAsync(); // so entity id gets resolved!

          workflow = FindOrCreate(
            entity.Id,
            param.Instance.Type,
            param.Instance.State,
            entity.Assignee
          );

          EnsureWorkflowVariables(workflow, param);

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            await PersistWorkflow(workflow, param);
            await _context.SaveChangesAsync();

            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();

          _logger.LogError(
            $"Error in triggering: {param.Instance.Type}, EntityId: {entity.Id}",
            ex.StackTrace
          );
        }
      }

      return result;
    }

    public IWorkflow Find(int id, Type type)
    {
      return (IWorkflow)_context.Workflows.Find(type, id);
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = this._workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }

    private Workflow FindOrCreate(int id, string type, string state, string assignee)
    {
      var workflow = this._context.Workflows.Include(_ => _.WorkflowVariables)
        .SingleOrDefault(w => w.CorrelationId == id && w.Type == type);
      if (workflow == null)
      {
        workflow = Workflow.Create(id, type, state, assignee);
        this._context.Workflows.Add(workflow);
      }

      return workflow;
    }

    private void EnsureWorkflowVariables(Workflow workflow, TriggerParam param)
    {
      if (workflow.WorkflowVariables.Count > 0)
      {
        foreach (var workflowVariable in workflow.WorkflowVariables)
        {
          var type = Type.GetType(workflowVariable.Type);
          var variable = JsonConvert.DeserializeObject(workflowVariable.Content, type);
          var workflowVariableBase = variable as WorkflowVariableBase;
          if (workflowVariableBase != null)
          {
            param.Variables.Add(KeyBuilder.ToKey(type), variable as WorkflowVariableBase);
          }
        }
      }
    }

    private async Task PersistWorkflow(
      Workflow workflow,
      TriggerParam triggerParam,
      DateTime? dueDate = null
    )
    {
      if (workflow == null) throw new ArgumentNullException(nameof(workflow));

      if (triggerParam.Variables != null && triggerParam.HasVariables)
      {
        foreach (var v in triggerParam.Variables)
        {
          var variable = workflow.WorkflowVariables
            .FirstOrDefault(_ => _.Type == v.Key);

          if (variable != null) {
            variable.Content = JsonConvert.SerializeObject(v.Value);
          } 
          else 
          {
            variable = new WorkflowVariable 
            {
              Type = KeyBuilder.ToKey(v.Value.GetType()),
              Content = JsonConvert.SerializeObject(v.Value)
            };
            workflow.WorkflowVariables.Add(variable);
          }
        }
      }

      var entityWorkflow = triggerParam.Instance as IEntityWorkflow;
      if (entityWorkflow != null)
      {
        workflow.Type = entityWorkflow.Type;
        workflow.State = entityWorkflow.State;
        workflow.Assignee = entityWorkflow.Assignee;
      }

      if (await WorkflowIsCompleted(triggerParam))
      {
        workflow.Completed = SystemTime.Now();
      }

      workflow.DueDate = dueDate;
    }

    private async Task<bool> WorkflowIsCompleted(TriggerParam triggerParam)
    {
      var triggerResults
        = await this.GetTriggersAsync(triggerParam.Instance, triggerParam.Variables);

      return triggerResults.Count() == 0;
    }
  }
}