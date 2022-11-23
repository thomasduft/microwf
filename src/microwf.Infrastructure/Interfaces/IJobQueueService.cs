using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public interface IJobQueueService
  {
    /// <summary>
    /// Enqueues a work item.
    /// </summary>
    /// <param name="workItem"></param>
    Task Enqueue(WorkItem workItem);

    /// <summary>
    /// Processes work items.
    /// </summary>
    /// <returns></returns>
    Task ProcessItemsAsync();

    /// <summary>
    /// Resumes persisted work items.
    /// </summary>
    Task ResumeWorkItems();

    /// <summary>
    /// Persists running work items.
    /// </summary>
    /// <returns></returns>
    Task PersistWorkItemsAsync();

    /// <summary>
    /// Returns a snapshot of the current queued work items.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Domain.WorkItemDto> GetSnapshot();
  }
}