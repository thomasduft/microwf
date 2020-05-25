namespace tomware.Microwf.Domain
{
  public interface IWorkflowDefinitionViewModelCreator
  {
    /// <summary>
    /// Creates the WorkflowDefinitionViewModel based on the workflow type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    WorkflowDefinitionViewModel CreateViewModel(string type);
  }
}
