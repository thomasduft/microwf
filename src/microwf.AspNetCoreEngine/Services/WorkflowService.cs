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
    Task<PaginatedList<WorkflowViewModel>> GetWorkflowsAsync(PagingParameters pagingParameters);

    Task<PaginatedList<WorkflowViewModel>> GetMyWorkflowsAsync(PagingParameters pagingParameters);

    Task<WorkflowViewModel> GetAsync(int id);

    Task<IEnumerable<WorkflowHistory>> GetHistory(int id);

    Task<IEnumerable<WorkflowVariable>> GetVariables(int id);

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

    public async Task<PaginatedList<WorkflowViewModel>> GetWorkflowsAsync(
      PagingParameters pagingParameters
    )
    {
      var count = _context.Workflows.Count();

      var instances = await _context.Workflows
        .OrderByDescending(w => w.Id)
        .Skip(pagingParameters.GetSkipCount())
        .Take(pagingParameters.PageSize)
        .ToListAsync();

      var items = instances.Select(i => ToWorkflowViewModel(i));

      return new PaginatedList<WorkflowViewModel>(
        items,
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<PaginatedList<WorkflowViewModel>> GetMyWorkflowsAsync(
      PagingParameters pagingParameters
    )
    {
      var definitions = GetWorkflowDefinitions();

      var baseQuery = _context.Workflows
        .Where(w => definitions.Select(d => d.Type).Distinct().Contains(w.Type)
          && w.Assignee == _userContext.UserName);

      var count = baseQuery.Count();

      var instances = await baseQuery
        .OrderByDescending(w => w.Id)
        .Skip(pagingParameters.GetSkipCount())
        .Take(pagingParameters.PageSize)
        .ToListAsync();

      var items = instances.Select(w => ToWorkflowViewModel(w));

      return new PaginatedList<WorkflowViewModel>(
        items,
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<WorkflowViewModel> GetAsync(int id)
    {
      var workflow = await _context.Workflows.FindAsync(id);
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      return ToWorkflowViewModel(workflow);
    }

    public async Task<IEnumerable<WorkflowHistory>> GetHistory(int id)
    {
      var workflow = await _context.Workflows
        .Include(h => h.WorkflowHistories)
        .Where(w => w.Id == id).FirstAsync();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      return workflow.WorkflowHistories.OrderByDescending(h => h.Created);
    }

    public async Task<IEnumerable<WorkflowVariable>> GetVariables(int id)
    {
      var workflow = await _context.Workflows
       .Include(v => v.WorkflowVariables)
       .Where(w => w.Id == id).FirstAsync();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      return workflow.WorkflowVariables.OrderBy(v => v.Type);
    }

    // TODO: add paging!
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

    private WorkflowViewModel ToWorkflowViewModel(Workflow w)
    {
      var model = _viewModelCreator.CreateViewModel(w.Type);

      return new WorkflowViewModel
      {
        Id = w.Id,
        Type = w.Type,
        State = w.State,
        Title = model.Title,
        Description = model.Description,
        Assignee = w.Assignee,
        Started = w.Started,
        Completed = w.Completed
      };
    }
  }
}
