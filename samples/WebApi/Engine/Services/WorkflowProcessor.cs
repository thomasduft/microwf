using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microbus.Core;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  // TODO: remove IMessageHandler in own class
  public class WorkflowProcessor : BackgroundService, IMessageHandler<WorkItem>
  {
    private readonly ILogger<WorkflowProcessor> _logger;

    private readonly IJobQueueService _jobQueueService;

    public WorkflowProcessor(
      ILogger<WorkflowProcessor> logger,
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        await _jobQueueService.TriggerAsync();

        // TODO: delay as WorkflowProcessorOptions property
        await Task.Delay(5000, stoppingToken);
      }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogTrace($"Stopping WorkflowProcessor...");

      await _jobQueueService.PersistWorkItemsAsync();

      await Task.CompletedTask;
    }
  }
}
