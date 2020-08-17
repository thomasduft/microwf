using tomware.Microwf.Domain;

namespace tomware.Microwf.Tests.Integration.Utils
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