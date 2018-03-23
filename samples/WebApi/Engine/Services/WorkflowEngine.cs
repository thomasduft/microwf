using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowEngine
  {
    IEnumerable<TriggerResult> GetTriggers(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    );

    TriggerResult CanTrigger(TriggerParam param);

    TriggerResult Trigger(TriggerParam param);
  }

  public class WorkflowEngine<TContext> : IWorkflowEngine where TContext : DbContext
  {
    private readonly ILogger<WorkflowEngine<TContext>> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IWorkflowContextService _workflowContextService;
    private readonly TContext _context;

    public WorkflowEngine(
      TContext context,
      ILoggerFactory loggerFactory,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IWorkflowContextService workflowContextService
    )
    {
      this._context = context ?? throw new ArgumentNullException(nameof(context));
      this._logger = loggerFactory.CreateLogger<WorkflowEngine<TContext>>();
      this._workflowDefinitionProvider = workflowDefinitionProvider;
      this._workflowContextService = workflowContextService;
    }

    public TriggerResult CanTrigger(TriggerParam param)
    {
      if (param == null) throw new InvalidOperationException(nameof(param));

      var execution = GetExecution(param.Instance.Type);

      return execution.CanTrigger(param);
    }

    public IEnumerable<TriggerResult> GetTriggers(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    )
    {
      if (instance == null) throw new InvalidOperationException(nameof(instance));

      var execution = GetExecution(instance.Type);

      return execution.GetTriggers(instance, variables);
    }

    public TriggerResult Trigger(TriggerParam param)
    {
      if (param == null) throw new InvalidOperationException(nameof(param));

      TriggerResult result = null;

      using (var transaction = this._context.Database.BeginTransaction())
      {
        int? id = null;
        try
        {
          var execution = GetExecution(param.Instance.Type);

          WorkflowContext workflowContext = null;
          var workflow = param.Instance as IEntityWorkflow;
          if (workflow != null)
          {
            id = workflow.Id;
            workflowContext = GetContext(id.Value, param);
          }

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            if (id.HasValue)
            {
              this.PersistContext(id.Value, param.Variables);
            }

            this._context.SaveChanges();
            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();

          this._logger.LogError($"Error in triggering: {param.Instance.Type}, EntityId: {id}", ex);
        }
      }

      return result;
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = this._workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }

    private WorkflowContext GetContext(int id, TriggerParam triggerParam)
    {
      return this._workflowContextService.FindOrCreate(id, triggerParam);
    }

    private void PersistContext(
      int id,
      Dictionary<string, WorkflowVariableBase> variables
    )
    {
      this._workflowContextService.Update(id, variables);
    }
  }
}