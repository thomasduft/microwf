using System;
using System.Collections.Generic;
using tomware.Microwf;
using WebApi.Domain;

namespace WebApi.Services
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

  public class WorkflowEngine : IWorkflowEngine
  {
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly DomainContext _domainContext;

    public WorkflowEngine(
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      DomainContext domainContext
    )
    {
      _workflowDefinitionProvider = workflowDefinitionProvider;
      _domainContext = domainContext;
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

      var execution = GetExecution(param.Instance.Type);

      // using (var transaction = _domainContext.Database.BeginTransaction())
      // {
      var result = execution.Trigger(param);
      if (!result.IsAborted)
      {
        _domainContext.SaveChanges();
        // transaction.Commit();
      }

      return result;
      // }
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }
  }
}