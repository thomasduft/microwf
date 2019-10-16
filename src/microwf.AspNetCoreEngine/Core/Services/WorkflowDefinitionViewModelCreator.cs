using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace tomware.Microwf.Engine
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

  public class ConfigurationWorkflowDefinitionViewModelCreator
    : IWorkflowDefinitionViewModelCreator
  {
    private readonly WorkflowConfiguration _workflowConfiguration;

    public ConfigurationWorkflowDefinitionViewModelCreator(
      IOptions<WorkflowConfiguration> workflows
    )
    {
      _workflowConfiguration = workflows.Value;
    }

    public WorkflowDefinitionViewModel CreateViewModel(string type)
    {
      var workflowType = _workflowConfiguration
        .Types
        .First(t => t.Type == type);

      return new WorkflowDefinitionViewModel
      {
        Type = workflowType.Type,
        Title = workflowType.Title,
        Description = workflowType.Description,
        Route = workflowType.Route
      };
    }
  }
}
