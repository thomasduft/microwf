using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class WorkItemService : IWorkItemService
  {
    private readonly IWorkItemRepository repository;

    public WorkItemService(IWorkItemRepository repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PaginatedList<Domain.WorkItemDto>> GetUpCommingsAsync(
      PagingParameters pagingParameters
    )
    {
      var now = SystemTime.Now();

      var count = await this.repository
        .CountAsync(new WorkItemDueDateGreaterThanNowCount(now));

      var items = await this.repository
        .ListAsync(new WorkItemDueDateGreaterThanNowOrderedPaginated(
          now,
          pagingParameters.SkipCount,
          pagingParameters.PageSize
        ));

      return new PaginatedList<Domain.WorkItemDto>(
        ObjectMapper.ToWorkItemViewModelList(items),
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<PaginatedList<Domain.WorkItemDto>> GetFailedAsync(
      PagingParameters pagingParameters
    )
    {
      var count = await this.repository
        .CountAsync(new WorkItemRetryLimitHitCount(WorkItem.WORKITEM_RETRIES));

      var items = await this.repository
        .ListAsync(new WorkItemRetryLimitHitCountOrderedPaginated(
          WorkItem.WORKITEM_RETRIES,
          pagingParameters.SkipCount,
          pagingParameters.PageSize
        ));

      return new PaginatedList<Domain.WorkItemDto>(
        ObjectMapper.ToWorkItemViewModelList(items),
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync()
    {
      var now = SystemTime.Now();

      return await this.repository
        .ListAsync(new WorkItemFilterForItemsToResume(
          now,
          WorkItem.WORKITEM_RETRIES
        ));
    }

    public async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      await this.repository.PersistWorkItemsAsync(items);
    }

    public async Task Reschedule(WorkItemDto model)
    {
      await this.repository.Reschedule(model);
    }

    public async Task DeleteAsync(int id)
    {
      await this.repository.RemoveAsync(id);
    }
  }
}