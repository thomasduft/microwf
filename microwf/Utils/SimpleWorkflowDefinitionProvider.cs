using microwf.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace microwf.Utils
{
  public class SimpleWorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private List<IWorkflowDefinition> _workflowDefinitions = null;

    private static SimpleWorkflowDefinitionProvider _instance;

    public static SimpleWorkflowDefinitionProvider Instance
    {
      get
      {
        if (_instance == null) _instance = new SimpleWorkflowDefinitionProvider();

        return _instance;
      }
    }

    private SimpleWorkflowDefinitionProvider()
    {
      // so it remains a singleton
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
      => _workflowDefinitions.Add(workflowDefinition);

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return _workflowDefinitions.First
              (w => w.WorkflowType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
