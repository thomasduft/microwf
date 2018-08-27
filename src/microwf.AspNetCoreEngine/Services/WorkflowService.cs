using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowService
  {
    /// <summary>
    /// Returns a list of workflow definitions that exist in the system.
    /// </summary>
    /// <returns></returns>
    IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions();

    /// <summary>
    /// Returns the dot -> diagraph notation for the given workflow type. 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    string Dot(string type);
  }

  public class WorkflowService : IWorkflowService
  {
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IUserWorkflowMappingService _userWorkflowMappingService;
    private readonly IWorkflowDefinitionViewModelCreator _viewModelCreator;

    public WorkflowService(
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserWorkflowMappingService userWorkflowMappingService,
      IWorkflowDefinitionViewModelCreator viewModelCreator
    )
    {
      _workflowDefinitionProvider = workflowDefinitionProvider;
      _userWorkflowMappingService = userWorkflowMappingService;
      _viewModelCreator = viewModelCreator;
    }

    public IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions()
    {
      var workflowDefinitions = _workflowDefinitionProvider.GetWorkflowDefinitions();

      workflowDefinitions = _userWorkflowMappingService.Filter(workflowDefinitions);

      return workflowDefinitions.Select(d => _viewModelCreator.CreateViewModel(d.Type));
    }

    public string Dot(string type)
    {
      if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

      var workflowDefinition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return workflowDefinition.ToDot();
    }
  }
}
