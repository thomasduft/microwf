using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  // TODO Work with DTO object -> WorkItemInfo/Dto
  public interface IWorkItemService
  {
    Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync();

    Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

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

    public async Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync()
    {
      return await _context.WorkItems
        .Where(_ => _.Retries <= 3)
        .ToListAsync<WorkItem>();
    }

    public async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      var ids = items.Select(_ => _.Id).ToArray();
      var existingItems = await _context.WorkItems
        .Where(_ => ids.Contains(_.Id))
        .ToListAsync<WorkItem>();

      // updates
      _context.WorkItems.UpdateRange(items.Intersect(existingItems));

      // new items
      _context.WorkItems.AddRange(items.Except(existingItems));

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
}