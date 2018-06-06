using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowDefinitionViewModelCreator
  {
    WorkflowDefinitionViewModel CreateViewModel(string type);
  }

  public class ConfigurationWorkflowDefinitionViewModelCreator
    : IWorkflowDefinitionViewModelCreator
  {
    private readonly WorkflowConfiguration _workflows;

    public ConfigurationWorkflowDefinitionViewModelCreator(
      IOptions<WorkflowConfiguration> workflows
    )
    {
      this._workflows = workflows.Value;
    }

    public WorkflowDefinitionViewModel CreateViewModel(string type)
    {
      var workflow = this._workflows.Types.FirstOrDefault(_ => _.Type == type);

      return new WorkflowDefinitionViewModel
      {
        Type = workflow.Type,
        Title = workflow.Title,
        Route = workflow.Route,
        Description = workflow.Description
      };
    }
  }
}
