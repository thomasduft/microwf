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
    Task<PaginatedList<WorkItem>> GetUpCommingsAsync(PagingParameters pagingParameters);

    /// <summary>
    /// Returns a list of failed work items.
    /// </summary>
    /// <returns></returns>
    Task<PaginatedList<WorkItem>> GetFailedAsync(PagingParameters pagingParameters);

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

    /// <summary>
    /// Reschedules an existing WorkItem.
    /// </summary>
    /// <returns></returns>
    Task<int> Reschedule(WorkItemViewModel model);
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

    public async Task<PaginatedList<WorkItem>> GetUpCommingsAsync(PagingParameters pagingParameters)
    {
      var now = SystemTime.Now();

      var count = _context.WorkItems
        .Where(wi => wi.DueDate > now)
        .Count();

      var items = await _context.WorkItems
        .Where(wi => wi.DueDate > now)
        .OrderBy(wi => wi.DueDate)
        .Skip(pagingParameters.SkipCount)
        .Take(pagingParameters.PageSize)
        .AsNoTracking()
        .ToListAsync<WorkItem>();

      return new PaginatedList<WorkItem>(
        items,
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<PaginatedList<WorkItem>> GetFailedAsync(PagingParameters pagingParameters)
    {
      var count = _context.WorkItems
        .Where(wi => wi.Retries > Constants.WORKITEM_RETRIES)
        .Count();

      var items = await _context.WorkItems
        .Where(wi => wi.Retries > Constants.WORKITEM_RETRIES)
        .OrderBy(wi => wi.DueDate)
        .AsNoTracking()
        .ToListAsync<WorkItem>();

      return new PaginatedList<WorkItem>(
        items,
        count,
        pagingParameters.PageIndex,
        pagingParameters.PageSize
      );
    }

    public async Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync()
    {
      var now = SystemTime.Now();

      return await _context.WorkItems
        .Where(wi => wi.Retries <= Constants.WORKITEM_RETRIES
        && wi.DueDate <= now)
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

    public async Task<int> Reschedule(WorkItemViewModel model)
    {
      var item = await _context.WorkItems.FindAsync(model.Id);

      item.Retries = Constants.WORKITEM_RETRIES; // so it reschedules only once!
      if (model.DueDate.HasValue)
      {
        item.DueDate = model.DueDate.Value;
      }

      _context.WorkItems.Update(item);

      return await _context.SaveChangesAsync();
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