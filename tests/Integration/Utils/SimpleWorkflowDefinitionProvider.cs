using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Tests.Integration.Utils
{
  public class SimpleWorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private List<IWorkflowDefinition> workflowDefinitions = null;

    private static SimpleWorkflowDefinitionProvider instance;

    public static SimpleWorkflowDefinitionProvider Instance
    {
      get
      {
        if (instance == null) instance = new SimpleWorkflowDefinitionProvider();

        return instance;
      }
    }

    internal void Invalidate()
    {
      this.workflowDefinitions = new List<IWorkflowDefinition>();
    }

    public SimpleWorkflowDefinitionProvider()
    {
      this.workflowDefinitions = new List<IWorkflowDefinition>();
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
      => this.workflowDefinitions.Add(workflowDefinition);

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return this.workflowDefinitions.First(w => w.Type == type);
    }

    public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions()
    {
      return this.workflowDefinitions;
    }
  }
}