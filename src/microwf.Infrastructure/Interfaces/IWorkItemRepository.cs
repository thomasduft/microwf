using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public interface IWorkItemRepository : IAsyncRepository<WorkItem>
  {
    Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

    Task Reschedule(WorkItemDto model);

    Task RemoveAsync(int id);
  }
}