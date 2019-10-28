using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microbus.Core;

namespace tomware.Microwf.Engine
{
  public class EnqueueWorkItemMessageHandler : IMessageHandler<WorkItemMessage>
  {
    private readonly ILogger<EnqueueWorkItemMessageHandler> logger;

    private readonly IJobQueueService jobQueueService;

    public EnqueueWorkItemMessageHandler(
      ILogger<EnqueueWorkItemMessageHandler> logger,
      IJobQueueService jobQueueService
    )
    {
      this.logger = logger;
      this.jobQueueService = jobQueueService;
    }

    public async Task Handle(
      WorkItemMessage message,
      CancellationToken token = default(CancellationToken)
    )
    {
      this.logger.LogTrace($"Handle WorkItemMessage", message);

      await this.jobQueueService.Enqueue(WorkItem.Create(
        message.TriggerName,
        message.EntityId,
        message.WorkflowType
      ));
    }
  }
}
