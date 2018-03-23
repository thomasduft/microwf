using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowService
  {
    IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions();
    string Dot(string type);
  }

  public class WorkflowService : IWorkflowService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IWorkflowDefinitionViewModelCreator _viewModelCreator;

    public WorkflowService(
      IServiceProvider serviceProvider,
      IConfiguration configuration,
      IWorkflowDefinitionViewModelCreator viewModelCreator
    )
    {
      this._serviceProvider = serviceProvider;
      this._configuration = configuration;
      this._viewModelCreator = viewModelCreator;
    }

    public IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions()
    {
      var workflowDefinitions = this._serviceProvider.GetServices<IWorkflowDefinition>();

      return workflowDefinitions.Select(d => this._viewModelCreator.CreateViewModel(d.Type));
    }

    public string Dot(string type)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      var workflowDefinitions = this._serviceProvider.GetServices<IWorkflowDefinition>();
      var workflowDefinition = workflowDefinitions.FirstOrDefault(x => x.Type == type);

      return workflowDefinition.ToDot();
    }
  }
}
