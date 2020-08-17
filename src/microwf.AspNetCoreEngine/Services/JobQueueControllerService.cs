using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace tomware.Microwf.Engine
{
  public interface IJobQueueControllerService
  {
    Task<IEnumerable<WorkItemViewModel>> GetSnapshotAsync();

    Task<PaginatedList<WorkItemViewModel>> GetUpCommingsAsync(PagingParameters parameters);

    Task<PaginatedList<WorkItemViewModel>> GetFailedAsync(PagingParameters parameters);

    Task Reschedule(WorkItemViewModel model);
  }

  public class JobQueueControllerService : IJobQueueControllerService
  {
    private readonly IJobQueueService service;
    private readonly IWorkItemService workItemService;

    public JobQueueControllerService(
      IJobQueueService service,
      IWorkItemService workItemService
    )
    {
      this.service = service;
      this.workItemService = workItemService;
    }

    public async Task<IEnumerable<WorkItemViewModel>> GetSnapshotAsync()
    {
      var result = this.service.GetSnapshot();

      return await Task.FromResult(result.Select(x => ToViewModel(x)));
    }

    public async Task<PaginatedList<WorkItemViewModel>> GetUpCommingsAsync(
      PagingParameters parameters
    )
    {
      var result = await this.workItemService.GetUpCommingsAsync(parameters);

      return new PaginatedList<WorkItemViewModel>(
        result.Select(x => ToViewModel(x)),
        result.AllItemsCount,
        parameters.PageIndex,
        parameters.PageSize
      );
    }

    public async Task<PaginatedList<WorkItemViewModel>> GetFailedAsync(
      PagingParameters parameters
    )
    {
      var result = await this.workItemService.GetFailedAsync(parameters);

      return new PaginatedList<WorkItemViewModel>(
        result.Select(x => ToViewModel(x)),
        result.AllItemsCount,
        parameters.PageIndex,
        parameters.PageSize
      );
    }

    public async Task Reschedule(WorkItemViewModel model)
    {
      await this.workItemService.Reschedule(new Infrastructure.WorkItemDto
      {
        Id = model.Id,
        DueDate = model.DueDate
      });
    }

    private WorkItemViewModel ToViewModel(Domain.WorkItemDto dto)
    {
      return PropertyMapper<Domain.WorkItemDto, WorkItemViewModel>.From(dto);
      // return new WorkItemViewModel
      // {
      //   Id = dto.Id,
      //   TriggerName = dto.TriggerName,
      //   EntityId = dto.EntityId,
      //   WorkflowType = dto.WorkflowType,
      //   Retries = dto.Retries,
      //   Error = dto.Error,
      //   DueDate = dto.DueDate
      // };
    }
  }
}