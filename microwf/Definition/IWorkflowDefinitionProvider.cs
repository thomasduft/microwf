namespace microwf.Definition
{
  public interface IWorkflowDefinitionProvider
  {
    void RegisterWorkflow<T>(T workflowDefinition) where T : IWorkflowDefinition;

    IWorkflowDefinition GetWorkflowDefinition(string type);
  }
}
