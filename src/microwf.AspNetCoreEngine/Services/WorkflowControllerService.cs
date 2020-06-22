using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowControllerService
  {
    Task<PaginatedList<WorkflowViewModel>> GetWorkflowsAsync(WorkflowSearchPagingParameters parameters);
    Task<WorkflowViewModel> GetAsync(int id);
    Task<IEnumerable<WorkflowHistoryViewModel>> GetHistoryAsync(int id);
    Task<IEnumerable<WorkflowVariableViewModel>> GetVariablesAsync(int id);
    Task<IEnumerable<WorkflowDefinitionViewModel>> GetWorkflowDefinitionsAsync();
    Task<string> DotAsync(string type);
    Task<string> DotWithHistoryAsync(string type, int correlationId);
  }

  public class WorkflowControllerService : IWorkflowControllerService
  {
    private readonly IWorkflowService service;
    private readonly IJobQueueService jobQueueService;

    public WorkflowControllerService(
      IWorkflowService service,
      IJobQueueService jobQueueService
    )
    {
      this.service = service;
      this.jobQueueService = jobQueueService;
    }

    public async Task<PaginatedList<WorkflowViewModel>> GetWorkflowsAsync(
      WorkflowSearchPagingParameters parameters
    )
    {
      var result = await this.service.GetWorkflowsAsync(parameters);

      return new PaginatedList<WorkflowViewModel>(
        result.Select(x => ToViewModel(x)),
        result.AllItemsCount,
        parameters.PageIndex,
        parameters.PageSize
      );
    }

    public async Task<WorkflowViewModel> GetAsync(int id)
    {
      var result = await this.service.GetAsync(id);

      return ToViewModel(result);
    }

    public async Task<IEnumerable<WorkflowHistoryViewModel>> GetHistoryAsync(int id)
    {
      var result = await this.service.GetHistoryAsync(id);

      return result.Select(x => ToWorkflowHistoryViewModel(x));
    }

    public async Task<IEnumerable<WorkflowVariableViewModel>> GetVariablesAsync(int id)
    {
      var result = await this.service.GetVariablesAsync(id);

      return result.Select(x => ToWorkflowVariablesViewModel(x));
    }

    public async Task<IEnumerable<WorkflowDefinitionViewModel>> GetWorkflowDefinitionsAsync()
    {
      var result = this.service.GetWorkflowDefinitions();

      return await Task.FromResult(result.Select(x => ToWorkflowDefinitionViewModel(x)));
    }

    public async Task<string> DotAsync(string type)
    {
      var result = this.service.Dot(type);

      return await Task.FromResult(result);
    }

    public async Task<string> DotWithHistoryAsync(string type, int correlationId)
    {
      var result = await this.service.DotWithHistoryAsync(type, correlationId);

      return await Task.FromResult(result);
    }

    private WorkflowViewModel ToViewModel(WorkflowDto dto)
    {
      return PropertyMapper<WorkflowDto, WorkflowViewModel>.From(dto);
      // return new WorkflowViewModel
      // {
      //   Id = dto.Id,
      //   CorrelationId = dto.CorrelationId,
      //   Type = dto.Type,
      //   State = dto.State,
      //   Title = dto.Title,
      //   Description = dto.Description,
      //   Assignee = dto.Assignee,
      //   Route = dto.Route,
      //   Started = dto.Started,
      //   Completed = dto.Completed
      // };
    }

    private WorkflowHistoryViewModel ToWorkflowHistoryViewModel(WorkflowHistoryDto dto)
    {
      return PropertyMapper<WorkflowHistoryDto, WorkflowHistoryViewModel>.From(dto);
      // return new WorkflowHistoryViewModel
      // {
      //   Id = dto.Id,
      //   Created = dto.Created,
      //   FromState = dto.FromState,
      //   ToState = dto.ToState,
      //   UserName = dto.UserName,
      //   WorkflowId = dto.WorkflowId
      // };
    }

    private WorkflowVariableViewModel ToWorkflowVariablesViewModel(WorkflowVariableDto dto)
    {
      return PropertyMapper<WorkflowVariableDto, WorkflowVariableViewModel>.From(dto);
      // return new WorkflowVariableViewModel
      // {
      //   Id = dto.Id,
      //   Type = dto.Type,
      //   Content = dto.Content,
      //   WorkflowId = dto.WorkflowId
      // };
    }

    private WorkflowDefinitionViewModel ToWorkflowDefinitionViewModel(WorkflowDefinitionDto dto)
    {
      return PropertyMapper<WorkflowDefinitionDto, WorkflowDefinitionViewModel>.From(dto);
      // return new WorkflowDefinitionViewModel
      // {
      //   Type = dto.Type,
      //   Title = dto.Title,
      //   Description = dto.Description,
      //   Route = dto.Route
      // };
    }
  }
}