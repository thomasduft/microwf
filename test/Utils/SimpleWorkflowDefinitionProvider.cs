using tomware.Microwf.Core;
using System.Collections.Generic;
using System.Linq;
using System;

namespace microwf.Tests.Utils
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

    internal void Invalidate()
    {
      _workflowDefinitions = new List<IWorkflowDefinition>();
    }

    private SimpleWorkflowDefinitionProvider()
    {
      _workflowDefinitions = new List<IWorkflowDefinition>();
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
      => _workflowDefinitions.Add(workflowDefinition);

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return _workflowDefinitions.First(w => w.Type == type);
    }

    public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions()
    {
      return _workflowDefinitions;
    }
  }
}
