namespace tomware.Microwf.Domain
{
  public interface IWorkflowDefinitionDtoCreator
  {
    /// <summary>
    /// Creates the WorkflowDefinitionDto based on the workflow type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    WorkflowDefinitionDto Create(string type);
  }
}