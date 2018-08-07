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

    public Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      // TODO PersistWorkItemsAsync

      return Task.CompletedTask;
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