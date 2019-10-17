using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public class WorkItemRepository<TContext>
    : EfRepository<WorkItem>, IWorkItemRepository
    where TContext : EngineDbContext
  {
    public WorkItemRepository(TContext dbContext) : base(dbContext)
    {
    }

    public async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      var ids = items.Select(wi => wi.Id).ToArray();

      var existingItems = await this.DbContext.WorkItems
        .Where(wi => ids.Contains(wi.Id))
        .ToListAsync<WorkItem>();

      var comparer = new WorkItemComparer();

      // updates
      var updates = items.Intersect(existingItems, comparer);
      this.DbContext.WorkItems.UpdateRange(updates);

      // new items
      var inserts = items.Except(existingItems, comparer);
      this.DbContext.WorkItems.AddRange(inserts);

      await this.DbContext.SaveChangesAsync();
    }

    public async Task Reschedule(WorkItemInfoViewModel model)
    {
      var item = await this.DbContext.WorkItems.FindAsync(model.Id);

      item.Retries = Constants.WORKITEM_RETRIES; // so it reschedules only once!
      if (model.DueDate.HasValue)
      {
        item.DueDate = model.DueDate.Value;
      }

      this.DbContext.WorkItems.Update(item);

      await this.DbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
      var item = await this.GetByIdAsync(id);

      await this.DeleteAsync(item);
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
