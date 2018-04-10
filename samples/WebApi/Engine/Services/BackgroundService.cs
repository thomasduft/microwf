using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  // see: https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/multi-container-microservice-net-applications/background-tasks-with-ihostedservice

  public abstract class BackgroundService : IHostedService, IDisposable
  {
    private Task _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
      // Store the task we're executing
      _executingTask = ExecuteAsync(_stoppingCts.Token);

      // If the task is completed then return it, 
      // this will bubble cancellation and failure to the caller
      if (_executingTask.IsCompleted)
      {
        return _executingTask;
      }

      // Otherwise it's running
      return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
      // Stop called without start
      if (_executingTask == null)
      {
        return;
      }

      try
      {
        // Signal cancellation to the executing method
        _stoppingCts.Cancel();
      }
      finally
      {
        // Wait until the task completes or the stop token triggers
        await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                      cancellationToken));
      }

    }

    public virtual void Dispose()
    {
      _stoppingCts.Cancel();
    }
  }

  public abstract class ScopedBackgroundService : BackgroundService
  {
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopedBackgroundService(IServiceScopeFactory serviceScopeFactory) : base()
    {
      this._serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        using (var scope = this._serviceScopeFactory.CreateScope())
        {
          ProcessInScope(scope.ServiceProvider);
        }

        await Task.Delay(5000, stoppingToken);
      }
    }

    protected abstract void ProcessInScope(IServiceProvider serviceProvider);
  }

  public class WorkflowProcessor : ScopedBackgroundService
  {
    private readonly ILogger<WorkflowProcessor> _logger;

    public WorkflowProcessor(
      ILogger<WorkflowProcessor> logger,
      IServiceScopeFactory serviceScopeFactory
    ) : base(serviceScopeFactory)
    {
      _logger = logger;
    }

    protected override void ProcessInScope(IServiceProvider serviceProvider)
    {
      _logger.LogTrace($"Aquire instance of IWorkflowEngine.");
      var workflowEngine
        = serviceProvider.GetRequiredService<IWorkflowEngine>();
      var workflowDefinitionProvider
        = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      _logger.LogTrace($"WorkflowProcessor task is doing background work.");

      /// process list of WorkflowProcessorItem's
      //WorkflowProcessorItem item = new WorkflowProcessorItem();
      //EntityWorkflowDefinitionBase workflowDefinition
      //  = (EntityWorkflowDefinitionBase) workflowDefinitionProvider
      //                                    .GetWorkflowDefinition(item.WorkflowType);

      //IWorkflow workflow = workflowEngine.Find(item.EntityId, workflowDefinition.EntityType);
      //TriggerParam triggerParam = new TriggerParam(item.TriggerName, workflow);
      //TriggerResult triggerResult = workflowEngine.Trigger(triggerParam);
    }
  }
}
