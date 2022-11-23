using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace tomware.Microwf.Domain
{
  public class WorkflowConfiguration
  {
    public List<WorkflowType> Types { get; set; }
  }

  public class WorkflowType
  {
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Route { get; set; }
  }

  public class WorkflowDefinitionDtoCreator
    : IWorkflowDefinitionDtoCreator
  {
    private readonly WorkflowConfiguration workflowConfiguration;

    public WorkflowDefinitionDtoCreator(
      IOptions<WorkflowConfiguration> workflows
    )
    {
      this.workflowConfiguration = workflows.Value;
    }

    public WorkflowDefinitionDto Create(string type)
    {
      var workflowType = this.workflowConfiguration
        .Types
        .First(t => t.Type == type);

      return new WorkflowDefinitionDto
      {
        Type = workflowType.Type,
        Title = workflowType.Title,
        Description = workflowType.Description,
        Route = workflowType.Route
      };
    }
  }
}