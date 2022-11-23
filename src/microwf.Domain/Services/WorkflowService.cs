using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public class WorkflowService : IWorkflowService
  {
    private readonly IWorkflowRepository repository;
    private readonly IWorkflowDefinitionProvider workflowDefinitionProvider;
    private readonly IUserWorkflowMappingService userWorkflowMappingService;
    private readonly IWorkflowDefinitionDtoCreator viewModelCreator;

    public WorkflowService(
      IWorkflowRepository repository,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserWorkflowMappingService userWorkflowMappingService,
      IWorkflowDefinitionDtoCreator dtoCreator
    )
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
      this.workflowDefinitionProvider = workflowDefinitionProvider;
      this.userWorkflowMappingService = userWorkflowMappingService;
      this.viewModelCreator = dtoCreator;
    }

    public async Task<PaginatedList<WorkflowDto>> GetWorkflowsAsync(
      WorkflowSearchPagingParameters pagingParameters
    )
    {
      var count = await this.repository.CountAsync(new WorkflowCount());

      IReadOnlyList<Workflow> instances = null;
      if (pagingParameters.HasValues)
      {
        instances = await this.repository
          .ListAsync(new WorkflowInstancesFilterAndOrderedPaginated(
            pagingParameters
          ));
      }
      else
      {
        instances = await this.repository
          .ListAsync(new WorkflowInstancesOrderedPaginated(
            pagingParameters.SkipCount,
            pagingParameters.PageSize
          ));
      }

      var items = instances.Select(i => this.ToWorkflowDto(i));

      return new PaginatedList<WorkflowDto>(
        items,
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<WorkflowDto> GetAsync(int id)
    {
      var workflow = await this.repository.GetByIdAsync(id);
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      return this.ToWorkflowDto(workflow);
    }

    public async Task<IEnumerable<WorkflowHistoryDto>> GetHistoryAsync(int id)
    {
      var list = await this.repository.ListAsync(new WorkflowHistoryForWorkflow(id));
      var workflow = list.FirstOrDefault();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      var viewModels = workflow.WorkflowHistories.OrderByDescending(h => h.Created);

      return ObjectMapper.ToWorkflowHistoryViewModelList(viewModels);
    }

    public async Task<IEnumerable<WorkflowVariableDto>> GetVariablesAsync(int id)
    {
      var list = await this.repository.ListAsync(new WorkflowVariablesForWorkflow(id));
      var workflow = list.FirstOrDefault();
      if (workflow == null) throw new KeyNotFoundException(nameof(id));

      var viewModels = workflow.WorkflowVariables.OrderBy(v => v.Type);

      return ObjectMapper.ToWorkflowVariableViewModelList(viewModels);
    }

    public async Task<WorkflowDto> GetInstanceAsync(string type, int correlationId)
    {
      var list = await this.repository.ListAsync(new GetWorkflowInstance(type, correlationId));
      var workflow = list.FirstOrDefault();
      if (workflow == null) throw new KeyNotFoundException($"{type}, {correlationId}");

      return this.ToWorkflowDto(workflow);
    }

    public IEnumerable<WorkflowDefinitionDto> GetWorkflowDefinitions()
    {
      var workflowDefinitions = this.workflowDefinitionProvider.GetWorkflowDefinitions();

      workflowDefinitions = this.userWorkflowMappingService.Filter(workflowDefinitions);

      return workflowDefinitions.Select(d => this.viewModelCreator.Create(d.Type));
    }

    public string Dot(string type)
    {
      if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

      var workflowDefinition = this.workflowDefinitionProvider.GetWorkflowDefinition(type);

      return workflowDefinition.ToDot();
    }

    public async Task<string> DotWithHistoryAsync(string type, int correlationId)
    {
      if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

      var list = await this.repository
        .ListAsync(new GetWorkflowInstanceHistories(
          type,
          correlationId
        ));
      var workflow = list.FirstOrDefault();
      if (workflow == null) throw new KeyNotFoundException($"{type}, {correlationId}");

      var workflowDefinition = this.workflowDefinitionProvider.GetWorkflowDefinition(type);

      return workflowDefinition.ToDotWithHistory(workflow);
    }

    private WorkflowDto ToWorkflowDto(Workflow w)
    {
      var model = this.viewModelCreator.Create(w.Type);

      return new WorkflowDto
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