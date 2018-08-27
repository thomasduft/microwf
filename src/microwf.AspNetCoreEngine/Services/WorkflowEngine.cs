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
  public interface IWorkflowEngine
  {
    /// <summary>
    /// Returns the possible triggers for certain workflow instance.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    );

    /// <summary>
    /// Checks whether a certain trigger can be executed.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<TriggerResult> CanTriggerAsync(TriggerParam param);

    /// <summary>
    /// Triggers the desired trigger for a certain workflow instance.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<TriggerResult> TriggerAsync(TriggerParam param);

    /// <summary>
    /// Returns the desired workflow instance if existing.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
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

      var entity = param.Instance as IEntityWorkflow;
      if (entity == null)
      {
        // going the non EF way!
        _logger.LogTrace($@"Processing a non '${param.Instance.Type}' 
            entity instance.");

        var execution = GetExecution(param.Instance.Type);

        return execution.Trigger(param);
      }

      TriggerResult result = null;
      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {
          Workflow workflow = null;
          _logger.LogTrace($@"Processing an '${param.Instance.Type}' 
            entity instance with Id = ${entity.Id}.");

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
            ex,
            $"Error in triggering: {param.Instance.Type}, EntityId: {entity.Id}"
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

    public IWorkflow Find(int id, Type type)
    {
      return (IWorkflow)_context.Find(type, id);
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }

    private Workflow FindOrCreate(int id, string type, string state, string assignee)
    {
      var workflow = _context.Workflows.Include(v => v.WorkflowVariables)
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
      if (workflow.WorkflowVariables.Count > 0)
      {
        foreach (var workflowVariable in workflow.WorkflowVariables)
        {
          var type = KeyBuilder.FromKey(workflowVariable.Type);
          var variable = JsonConvert.DeserializeObject(workflowVariable.Content, type);
          var workflowVariableBase = variable as WorkflowVariableBase;
          if (workflowVariableBase != null)
          {
            var key = KeyBuilder.ToKey(type);
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
        = await GetTriggersAsync(triggerParam.Instance, triggerParam.Variables);

      return triggerResults.Count() == 0;
    }
  }
}