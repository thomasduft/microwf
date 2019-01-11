using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public interface IWorkItemService
  {
    /// <summary>
    /// Returns a list of upcomming work items.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<WorkItem>> GetUpCommingsAync();

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
    Task<int> DeleteAsync(int id);
  }

  public class WorkItemService<TContext> : IWorkItemService where TContext : EngineDbContext
  {
    private readonly EngineDbContext _context;
    private readonly ILogger<WorkItemService<TContext>> _logger;

    public WorkItemService(
      TContext context,
      ILogger<WorkItemService<TContext>> logger
    )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger;
    }

    public async Task<IEnumerable<WorkItem>> GetUpCommingsAync()
    {
      return await _context.WorkItems
       .Where(wi => wi.DueDate > SystemTime.Now())
       .OrderByDescending(wi => wi.DueDate)
       .ToListAsync<WorkItem>();
    }

    public async Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync()
    {
      return await _context.WorkItems
        .Where(wi => wi.Retries <= 3 && wi.DueDate <= SystemTime.Now())
        .ToListAsync<WorkItem>();
    }

    public async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      var ids = items.Select(wi => wi.Id).ToArray();
      var existingItems = await _context.WorkItems
        .Where(wi => ids.Contains(wi.Id))
        .ToListAsync<WorkItem>();

      var comparer = new WorkItemComparer();

      // updates
      var updates = items.Intersect(existingItems, comparer);
      _context.WorkItems.UpdateRange(updates);

      // new items
      var inserts = items.Except(existingItems, comparer);
      _context.WorkItems.AddRange(inserts);

      await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
      var item = await _context.WorkItems.FindAsync(id);
      if (item == null) return -1;

      _context.WorkItems.Remove(item);
      await _context.SaveChangesAsync();

      return item.Id;
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