using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public interface IWorkItemService
  {
    /// <summary>
    /// Returns a list of upcomming work items.
    /// </summary>
    /// <returns></returns>
    Task<PaginatedList<Domain.WorkItemDto>> GetUpCommingsAsync(PagingParameters pagingParameters);

    /// <summary>
    /// Returns a list of failed work items.
    /// </summary>
    /// <returns></returns>
    Task<PaginatedList<Domain.WorkItemDto>> GetFailedAsync(PagingParameters pagingParameters);

    /// <summary>
    /// Returns a list of persisted WorkItems.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync();

    /// <summary>
    /// Saves the WorkItem collection.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

    /// <summary>
    /// Deletes a persisted WorkItem by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Reschedules an existing WorkItem.
    /// </summary>
    /// <returns></returns>
    Task Reschedule(WorkItemDto model);
  }
}