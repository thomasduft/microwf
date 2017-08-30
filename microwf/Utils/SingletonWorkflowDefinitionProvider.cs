using microwf.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace microwf.Utils
{
  public class SingletonWorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private List<IWorkflowDefinition> _workflowDefinitions = null;

    private static SingletonWorkflowDefinitionProvider _instance;

    public static SingletonWorkflowDefinitionProvider Instance
    {
      get
      {
        if (_instance == null) _instance = new SingletonWorkflowDefinitionProvider();

        return _instance;
      }
    }

    private SingletonWorkflowDefinitionProvider()
    {
      // so it remains a singleton
    }

    public void RegisterWorkflow<T>(T workflowDefinition) where T : IWorkflowDefinition
      => _workflowDefinitions.Add(workflowDefinition);

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return _workflowDefinitions.First
              (w => w.WorkflowType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
