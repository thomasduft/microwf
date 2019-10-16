using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public class WorkItemService : IWorkItemService
  {
    private readonly IWorkItemRepository repository;

    public WorkItemService(IWorkItemRepository repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PaginatedList<WorkItemViewModel>> GetUpCommingsAsync(
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

      return new PaginatedList<WorkItemViewModel>(
        ViewModelMapper.ToWorkItemViewModelList(items),
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<PaginatedList<WorkItemViewModel>> GetFailedAsync(
      PagingParameters pagingParameters
    )
    {
      var count = await this.repository
        .CountAsync(new WorkItemRetryLimitHitCount(Constants.WORKITEM_RETRIES));

      var items = await this.repository
        .ListAsync(new WorkItemRetryLimitHitCountOrderedPaginated(
          Constants.WORKITEM_RETRIES,
          pagingParameters.SkipCount,
          pagingParameters.PageSize
        ));

      return new PaginatedList<WorkItemViewModel>(
        ViewModelMapper.ToWorkItemViewModelList(items),
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
          Constants.WORKITEM_RETRIES
        ));
    }

    public async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      await this.repository.PersistWorkItemsAsync(items);
    }

    public async Task Reschedule(WorkItemInfoViewModel model)
    {
      await this.repository.Reschedule(model);
    }

    public async Task DeleteAsync(int id)
    {
      await this.repository.DeleteAsync(id);
    }
  }

  public class WorkItemComparer : IEqualityComparer<WorkItem>
  {
    public bool Equals(WorkItem wm1, WorkItem wm2)
    {
      if (wm1.Id == wm2.Id)
      {
        return true;
      }

      return false;
    }

    public int GetHashCode(WorkItem workItem)
    {
      return workItem.Id.GetHashCode();
    }
  }
}