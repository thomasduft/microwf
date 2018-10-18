using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microbus.Core;

namespace tomware.Microwf.Engine
{
  public class EnqueueWorkItemMessageHandler : IMessageHandler<WorkItemMessage>
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
      WorkItemMessage item,
      CancellationToken token = default(CancellationToken)
    )
    {
      _logger.LogTrace($"Enqueue work item", item);

      WorkItem workItem = WorkItem.Create(item.TriggerName, item.EntityId, item.WorkflowType);
      _jobQueueService.Enqueue(workItem);

      await Task.CompletedTask;
    }
  }
}
