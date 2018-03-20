using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.AspNetCore
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
      _serviceProvider = serviceProvider;
      _configuration = configuration;
      _viewModelCreator = viewModelCreator;
    }

    public IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions()
    {
      var workflowDefinitions = this._serviceProvider.GetServices<IWorkflowDefinition>();

      return workflowDefinitions.Select(d => _viewModelCreator.CreateViewModel(d.Type));
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
