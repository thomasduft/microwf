using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Domain;

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

    IWorkflow Find(int id, Type type);
  }

  public class WorkflowEngine<TContext> : IWorkflowEngine where TContext : DbContext
  {
    private readonly DomainContext _context;
    private readonly ILogger<WorkflowEngine<TContext>> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;

    public WorkflowEngine(
      DomainContext context,
      ILogger<WorkflowEngine<TContext>> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider
    )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger;
      _workflowDefinitionProvider = workflowDefinitionProvider;
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

          var entity = param.Instance as IEntityWorkflow;
          WebApi.Domain.Workflow workflow = null;
          if (entity != null)
          {
            id = entity.Id;
            workflow = FindOrCreate(
              id.Value,
              param.Instance.Type,
              param.Instance.State,
              entity.Assignee
            );
            EnsureContext(param, workflow.WorkflowContext);
          }

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            if (id.HasValue)
            {
              PersistWorkflow(workflow, param);
            }

            _context.SaveChanges();
            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();

          _logger.LogError($"Error in triggering: {param.Instance.Type}, EntityId: {id}", ex);
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

    private WebApi.Domain.Workflow FindOrCreate(int id, string type, string state, string assignee)
    {
      var workflow = this._context.Workflows.Include(_ => _.WorkflowContext)
        .SingleOrDefault(w => w.CorrelationId == id && w.Type == type);
      if (workflow == null)
      {
        workflow = WebApi.Domain.Workflow.Create(id, type, state, assignee);
        this._context.Add(workflow);
      }

      return workflow;
    }

    private void EnsureContext(TriggerParam triggerParam, WorkflowContext ctx)
    {
      if (triggerParam == null) throw new ArgumentNullException(nameof(triggerParam));
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));

      if (!triggerParam.HasVariables && !string.IsNullOrWhiteSpace(ctx.Context))
      {
        var variables = JsonConvert
          .DeserializeObject<Dictionary<string, WorkflowVariableBase>>(ctx.Context);
        foreach (var variable in variables)
        {
          triggerParam.Variables.Add(variable.Key, variable.Value);
        }
      }
    }

    private void PersistWorkflow(
      WebApi.Domain.Workflow workflow,
      TriggerParam triggerParam,
      DateTime? dueDate = null
    )
    {
      if (workflow == null) throw new ArgumentNullException(nameof(workflow));

      if (triggerParam.Variables != null && triggerParam.HasVariables)
      {
        workflow.WorkflowContext.Context = JsonConvert.SerializeObject(triggerParam.Variables);
      }

      var entityWorkflow = triggerParam.Instance as IEntityWorkflow;
      if (entityWorkflow != null)
      {
        workflow.Type = entityWorkflow.Type;
        workflow.State = entityWorkflow.State;
        workflow.Assignee = entityWorkflow.Assignee;
      }

      workflow.DueDate = dueDate;
    }
  }
}