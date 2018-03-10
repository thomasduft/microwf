namespace tomware.Microwf
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
  }
}
