using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public class WorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private readonly IEnumerable<IWorkflowDefinition> _workflowDefinitions;

    public WorkflowDefinitionProvider(IEnumerable<IWorkflowDefinition> workflowDefinitions)
    {
      _workflowDefinitions = workflowDefinitions;
    }

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return _workflowDefinitions.First(t => t.Type == type);
    }

    public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions()
    {
      return _workflowDefinitions;
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
    {
      throw new NotImplementedException();
    }
  }
}
