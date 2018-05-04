using System.Collections.Generic;

namespace tomware.Microwf.Core
{
  public interface IWorkflowDefinitionProvider
  {
    /// <summary>
    /// Registers a workflow definition.
    /// </summary>
    /// <param name="workflowDefinition"></param>
    void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition);

    /// <summary>
    /// Returns a workflow definition.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IWorkflowDefinition GetWorkflowDefinition(string type);

    /// <summary>
    /// Returns a list of workflow definitions.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions();
  }
}
