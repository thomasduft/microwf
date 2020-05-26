using tomware.Microwf.Domain;

namespace microwf.Tests.Utils
{
  public class TestWorkflowDefinitionViewModelCreator : IWorkflowDefinitionDtoCreator
  {
    public WorkflowDefinitionDto Create(string type)
    {
      return new WorkflowDefinitionDto
      {
        Type = type,
        Title = "Title",
        Route = "Route",
        Description = "Description"
      };
    }
  }
}