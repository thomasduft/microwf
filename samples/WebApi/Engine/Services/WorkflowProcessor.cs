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
  // TODO: add common WorkItem Queue in order to process WorkItems
  public class WorkflowProcessor : BackgroundService, IMessageHandler<WorkItem>
  {
    private readonly ILogger<WorkflowProcessor> _logger;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WorkflowProcessor(
      ILogger<WorkflowProcessor> logger,
      IServiceScopeFactory serviceScopeFactory
    )
    {
      _logger = logger;
      _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(WorkItem message, CancellationToken token = default(CancellationToken))
    {
      _logger.LogTrace($"Adding WorkItem: {message.TriggerName} - {message.EntityId} - {message.WorkflowType}");

      await Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        using (var scope = this._serviceScopeFactory.CreateScope())
        {
          Process(scope.ServiceProvider);
        }

        // TODO: delay as WorkflowProcessorOptions property
        await Task.Delay(5000, stoppingToken);
      }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogTrace($"Stopping WorkflowProcessor...");

      // stop processing current items -> persist?!

      await Task.CompletedTask;
    }

    protected void Process(IServiceProvider serviceProvider)
    {
      _logger.LogTrace($"Aquire instance of IWorkflowEngine.");
      var workflowEngine
        = serviceProvider.GetRequiredService<IWorkflowEngine>();
      var workflowDefinitionProvider
        = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      _logger.LogTrace($"WorkflowProcessor task is doing background work.");

      /// process list of WorkflowProcessorItem's
      //WorkItem item = new WorkItem();
      //EntityWorkflowDefinitionBase workflowDefinition
      //  = (EntityWorkflowDefinitionBase) workflowDefinitionProvider
      //                                    .GetWorkflowDefinition(item.WorkflowType);

      //IWorkflow workflow = workflowEngine.Find(item.EntityId, workflowDefinition.EntityType);
      //TriggerParam triggerParam = new TriggerParam(item.TriggerName, workflow);
      //TriggerResult triggerResult = workflowEngine.Trigger(triggerParam);
    }
  }
}
