using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microbus.Core;

namespace tomware.Microwf.Engine
{
  // TODO introduce EnqueWorkItemMessage type
  public class EnqueueWorkItemMessageHandler : IMessageHandler<WorkItem>
  {
    private readonly ILogger<EnqueueWorkItemMessageHandler> _logger;

    private readonly IJobQueueService _jobQueueService;

    public EnqueueWorkItemMessageHandler(
      ILogger<EnqueueWorkItemMessageHandler> logger,
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
      _logger.LogTrace($"Enqueue work item", item);

      _jobQueueService.Enqueue(item);

      await Task.CompletedTask;
    }
  }
}
