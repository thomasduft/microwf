using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
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
      WorkflowSearchPagingParameters pagingParameters
    )
    {
      var count = _context.Workflows.Count();

      List<Workflow> instances = null;
      if (pagingParameters.HasValues)
      {
        // Specification Pattern ?!
        // see: https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core#implementing-the-specification-pattern
        instances = await _context.Workflows
          .Where(this.GetWhereClause(pagingParameters))
          .OrderByDescending(w => w.Id)
          .Skip(pagingParameters.SkipCount)
          .Take(pagingParameters.PageSize)
          .AsNoTracking()
          .ToListAsync();
      }
      else
      {
        instances = await _context.Workflows
          .OrderByDescending(w => w.Id)
          .Skip(pagingParameters.SkipCount)
          .Take(pagingParameters.PageSize)
          .AsNoTracking()
          .ToListAsync();
      }

      var items = instances.Select(i => ToWorkflowViewModel(i));

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

    public async Task<IEnumerable<WorkflowHistoryViewModel>> GetHistoryAsync(int id)
    {
      var workflow = await _context.Workflows
        .Include(h => h.WorkflowHistories)
        .Where(w => w.Id == id)
        .AsNoTracking()
        .FirstAsync();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      var viewModels = workflow.WorkflowHistories.OrderByDescending(h => h.Created);

      return ViewModelMapper.ToWorkflowHistoryViewModelList(viewModels);
    }

    public async Task<IEnumerable<WorkflowVariableViewModel>> GetVariablesAsync(int id)
    {
      var workflow = await _context.Workflows
        .Include(v => v.WorkflowVariables)
        .Where(w => w.Id == id)
        .AsNoTracking()
        .FirstAsync();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      var viewModels = workflow.WorkflowVariables.OrderBy(v => v.Type);

      return ViewModelMapper.ToWorkflowVariableViewModelList(viewModels);
    }

    public async Task<WorkflowViewModel> GetInstanceAsync(string type, int correlationId)
    {
      var workflow = await _context.Workflows
        .AsNoTracking()
        .FirstAsync(w => w.Type == type && w.CorrelationId == correlationId);
      if (workflow == null) throw new KeyNotFoundException($"{type}, {correlationId}");

      return ToWorkflowViewModel(workflow);
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

    public async Task<string> DotWithHistoryAsync(string type, int correlationId)
    {
      if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

      var workflow = await _context.Workflows
        .Include(h => h.WorkflowHistories)
        .AsNoTracking()
        .FirstAsync(w => w.Type == type && w.CorrelationId == correlationId);
      if (workflow == null) throw new KeyNotFoundException($"{type}, {correlationId}");

      var workflowDefinition = _workflowDefinitionProvider.GetWorkflowDefinition(type);

      return workflowDefinition.ToDotWithHistory(workflow);
    }

    private Expression<Func<Workflow, bool>> GetWhereClause(
      WorkflowSearchPagingParameters pagingParameters
    )
    {
      var predicate = PredicateBuilder.True<Workflow>();
      if (pagingParameters.HasType)
      {
        predicate = predicate
          .And(w => w.Type.ToLowerInvariant()
            .StartsWith(pagingParameters.Type.ToLowerInvariant()));
      }
      if (pagingParameters.HasCorrelationId)
      {
        predicate = predicate
          .And(w => w.CorrelationId == pagingParameters.CorrelationId);
      }
      if (pagingParameters.HasAssignee)
      {
        predicate = predicate
          .And(w => w.Assignee.ToLowerInvariant()
            .StartsWith(pagingParameters.Assignee.ToLowerInvariant()));
      }

      return predicate;
    }

    private WorkflowViewModel ToWorkflowViewModel(Workflow w)
    {
      var model = _viewModelCreator.CreateViewModel(w.Type);

      return new WorkflowViewModel
      {
        Id = w.Id,
        CorrelationId = w.CorrelationId,
        Type = w.Type,
        State = w.State,
        Title = model.Title,
        Description = model.Description,
        Assignee = w.Assignee,
        Started = w.Started,
        Completed = w.Completed,
        Route = model.Route
      };
    }
  }
}
