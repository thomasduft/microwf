using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microbus.Core;

namespace tomware.Microwf.Engine
{
  public class WorkItemMessageHandler : IMessageHandler<WorkItem>
  {
    private readonly ILogger<WorkItemMessageHandler> _logger;

    private readonly IJobQueueService _jobQueueService;

    public WorkItemMessageHandler(
      ILogger<WorkItemMessageHandler> logger,
      IJobQueueService jobQueueService
    )
    {
      _logger = logger;
      _jobQueueService = jobQueueService;
    }

    public async Task Handle(
      WorkItem item,
      CancellationToken token = default(CancellationToken)
    )
    {
      _logger.LogTrace($"Adding work item", item);

      _jobQueueService.Enqueue(item);

      await Task.CompletedTask;
    }
  }
}
