using Microsoft.Extensions.Configuration;

namespace tomware.Microwf.AspNetCore
{
  public interface IWorkflowDefinitionViewModelCreator
  {
    WorkflowDefinitionViewModel CreateViewModel(string type);
  }

  public class ConfigurationWorkflowDefinitionViewModelCreator
    : IWorkflowDefinitionViewModelCreator
  {
    private readonly IConfiguration _configuration;

    public ConfigurationWorkflowDefinitionViewModelCreator(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public WorkflowDefinitionViewModel CreateViewModel(string type)
    {
      var startUrl = this._configuration[$"Workflows:{type}:StartUrl"];
      var description = this._configuration[$"Workflows:{type}:Description"];

      return new WorkflowDefinitionViewModel
      {
        Type = type,
        StartUrl = startUrl,
        Description = description
      };
    }
  }
}
