using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public class WorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private readonly IEnumerable<IWorkflowDefinition> workflowDefinitions;

    public WorkflowDefinitionProvider(IEnumerable<IWorkflowDefinition> workflowDefinitions)
    {
      this.workflowDefinitions = workflowDefinitions;
    }

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return this.workflowDefinitions.First(t => t.Type == type);
    }

    public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions()
    {
      return this.workflowDefinitions;
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
    {
      throw new NotImplementedException();
    }
  }
}