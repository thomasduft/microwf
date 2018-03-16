using System;
using tomware.Microwf;
using WebApi.Workflows;

namespace WebApi.Services
{
  public interface IWorkflowActivator
  {
    IWorkflow Create(string workflowType);
  }

  public class WorkflowActivator : IWorkflowActivator
  {
    private IWorkflowDefinitionProvider _workflowDefinitionProvider;

    public WorkflowActivator(IWorkflowDefinitionProvider workflowDefinitionProvider)
    {
      _workflowDefinitionProvider = workflowDefinitionProvider;
    }

    public IWorkflow Create(string workflowType)
    {
      var workflowDefinition = _workflowDefinitionProvider.GetWorkflowDefinition(workflowType);
      var workflowFactory = workflowDefinition as IWorkflowFactory;
      if (workflowFactory == null) throw new InvalidOperationException(nameof(workflowFactory));

      return workflowFactory.Create();
    }
  }
}