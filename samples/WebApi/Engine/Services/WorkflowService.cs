using Microsoft.Extensions.Configuration;
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
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IUserWorkflowMappingService _userWorkflowDefinitionService;
    private readonly IConfiguration _configuration;
    private readonly IWorkflowDefinitionViewModelCreator _viewModelCreator;

    public WorkflowService(
      IConfiguration configuration,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserWorkflowMappingService userWorkflowDefinitionService,
      IWorkflowDefinitionViewModelCreator viewModelCreator
    )
    {
      this._workflowDefinitionProvider = workflowDefinitionProvider;
      this._userWorkflowDefinitionService = userWorkflowDefinitionService;
      this._configuration = configuration;
      this._viewModelCreator = viewModelCreator;
    }

    public IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions()
    {
      var workflowDefinitions = this._workflowDefinitionProvider.GetWorkflowDefinitions();

      workflowDefinitions = this._userWorkflowDefinitionService.Filter(workflowDefinitions);

      return workflowDefinitions.Select(d => this._viewModelCreator.CreateViewModel(d.Type));
    }

    public string Dot(string type)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));

      var workflowDefinition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return workflowDefinition.ToDot();
    }
  }
}
