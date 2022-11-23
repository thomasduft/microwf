using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace tomware.Microwf.Infrastructure
{
  public class ProcessorConfiguration
  {
    public bool Enabled { get; set; }
    public int Interval { get; set; }
  }

  public class WorkflowProcessor : BackgroundService
  {
    private readonly ILogger<WorkflowProcessor> logger;

    private readonly ProcessorConfiguration options;

    private readonly IJobQueueService jobQueueService;

    public WorkflowProcessor(
      ILogger<WorkflowProcessor> logger,
      IOptions<ProcessorConfiguration> options,
      IJobQueueService jobQueueService
    )
    {
      this.logger = logger;
      this.options = options.Value;
      this.jobQueueService = jobQueueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        this.logger.LogTrace($"Triggering JobQueueService.ProcessItemsAsync");

        await this.jobQueueService.ResumeWorkItems();
        await this.jobQueueService.ProcessItemsAsync();

        await Task.Delay(this.options.Interval, stoppingToken);
      }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
      this.logger.LogTrace($"Stopping processor");

      await this.jobQueueService.PersistWorkItemsAsync();
    }
  }
}