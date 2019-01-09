using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public class WorkflowProcessor : BackgroundService
  {
    private readonly ILogger<WorkflowProcessor> _logger;

    private readonly ProcessorConfiguration _options;

    private readonly IJobQueueService _jobQueueService;

    public WorkflowProcessor(
      ILogger<WorkflowProcessor> logger,
      IOptions<ProcessorConfiguration> options,
      IJobQueueService jobQueueService
    )
    {
      _logger = logger;
      _options = options.Value;
      _jobQueueService = jobQueueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogTrace($"Triggering JobQueueService.ProcessItemsAsync");

        await _jobQueueService.ResumeWorkItems();
        await _jobQueueService.ProcessItemsAsync();

        await Task.Delay(_options.Interval, stoppingToken);
      }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogTrace($"Stopping processor");

      await _jobQueueService.PersistWorkItemsAsync();
    }
  }
}
