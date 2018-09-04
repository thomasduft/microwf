using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowService
  {
    Task<IEnumerable<WorkflowViewModel>> GetWorkflowsAsync();

    Task<IEnumerable<WorkflowViewModel>> GetMyWorkflowsAsync();

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

  public class WorkflowService<TContext> : IWorkflowService where TContext : EngineDbContext
  {
    private readonly EngineDbContext _context;
    private readonly ILogger<WorkflowService<TContext>> _logger;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;
    private readonly IUserWorkflowMappingService _userWorkflowMappingService;
    private readonly IWorkflowDefinitionViewModelCreator _viewModelCreator;
    private readonly IUserContextService _userContext;

    public WorkflowService(
      TContext context,
      ILogger<WorkflowService<TContext>> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserWorkflowMappingService userWorkflowMappingService,
      IWorkflowDefinitionViewModelCreator viewModelCreator,
      IUserContextService userContext
    )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger;
      _workflowDefinitionProvider = workflowDefinitionProvider;
      _userWorkflowMappingService = userWorkflowMappingService;
      _viewModelCreator = viewModelCreator;
      _userContext = userContext;
    }

    public async Task<IEnumerable<WorkflowViewModel>> GetWorkflowsAsync()
    {
      var instances = await _context.Workflows
        .OrderByDescending(w => w.Id)
        .ToListAsync();

      return instances.Select(i => ToWorkflowViewModel(i));
    }

    public async Task<IEnumerable<WorkflowViewModel>> GetMyWorkflowsAsync()
    {
      var definitions = GetWorkflowDefinitions();

      var instances = await _context.Workflows
        .Where(w => definitions.Select(d => d.Type).Distinct().Contains(w.Type)
          && w.Assignee == _userContext.UserName)
        .OrderByDescending(w => w.Id)
        .ToListAsync();

      return instances.Select(w => ToWorkflowViewModel(w));
    }

    private WorkflowViewModel ToWorkflowViewModel(Workflow w)
    {
      var title = _viewModelCreator.CreateViewModel(w.Type).Title;

      return new WorkflowViewModel
      {
        Id = w.Id,
        Type = w.Type,
        State = w.State,
        Title = title,
        Assignee = w.Assignee,
        Started = w.Started,
        Completed = w.Completed
      };
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
