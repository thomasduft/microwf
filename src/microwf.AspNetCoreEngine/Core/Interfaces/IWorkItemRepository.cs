using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public interface IWorkItemRepository : IAsyncRepository<WorkItem>
  {
    Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

    Task Reschedule(WorkItemInfoViewModel model);

    Task RemoveAsync(int id);
  }
}
