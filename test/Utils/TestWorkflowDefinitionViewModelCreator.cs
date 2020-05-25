using tomware.Microwf.Domain;

namespace microwf.Tests.Utils
{
  public class TestWorkflowDefinitionViewModelCreator : IWorkflowDefinitionViewModelCreator
  {
    public WorkflowDefinitionViewModel CreateViewModel(string type)
    {
      return new WorkflowDefinitionViewModel
      {
        Type = type,
        Title = "Title",
        Route = "Route",
        Description = "Description"
      };
    }
  }
}