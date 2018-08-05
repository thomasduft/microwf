using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IJobQueueService
  {
    void Enqueue(WorkItem workItem);

    WorkItem Dequeue();

    Task TriggerAsync();

    void ResumeWorkItems();

    Task PersistWorkItemsAsync();
  }

  public class JobQueueService : IJobQueueService
  {
    private readonly ILogger<JobQueueService> _logger;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentQueue<WorkItem> _items;

    public JobQueueService(
      ILogger<JobQueueService> logger,
      IServiceScopeFactory serviceScopeFactory
    )
    {
      _logger = logger;
      _serviceScopeFactory = serviceScopeFactory;

      _items = new ConcurrentQueue<WorkItem>();

      ResumeWorkItems();
    }

    public void Enqueue(WorkItem item)
    {
      _logger.LogTrace("Enqueue work item", item);
      if (item.Retries > 3)
      {
        _logger.LogInformation("Amount of retries exceeded", item);
        return;
      }

      _items.Enqueue(item);
    }

    public WorkItem Dequeue()
    {
      WorkItem item;
      if (_items.TryDequeue(out item))
      {
        _logger.LogTrace("Dequeued work item", item);
        return item;
      }

      return null;
    }

    public async Task TriggerAsync()
    {
      _logger.LogTrace("Triggering job queue for doing work");

      while (_items.Count != 0)
      {
        var item = Dequeue();
        if (item == null) continue;

        TriggerResult triggerResult = await ProcessItemAsync(item);
        if (triggerResult.HasErrors || triggerResult.IsAborted)
        {
          _logger.LogError(
            "ProcessingWorkItemFailed",
            string.Join('-', triggerResult.Errors),
            triggerResult
          );

          item.Retries++;
          this.Enqueue(item);
        }
      }

      await Task.CompletedTask;
    }

    public void ResumeWorkItems()
    {
      // TODO Resuming work items
      _logger.LogTrace("Resuming work items", _items);
    }

    public async Task PersistWorkItemsAsync()
    {
      // TODO Persisting work items
      _logger.LogTrace("Persisting work items", _items);

      await Task.CompletedTask;
    }

    private async Task<TriggerResult> ProcessItemAsync(WorkItem item)
    {
      _logger.LogTrace("Processing work item", item);

      using (var scope = this._serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;

        _logger.LogTrace($"Aquire instance of IWorkflowEngine.");
        var engine = serviceProvider.GetRequiredService<IWorkflowEngine>();
        var workflowDefinitionProvider
          = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();
        _logger.LogTrace($"WorkflowProcessor task is doing background work.");

        // process list of WorkflowProcessorItem's
        EntityWorkflowDefinitionBase workflowDefinition
         = (EntityWorkflowDefinitionBase)workflowDefinitionProvider
                                           .GetWorkflowDefinition(item.WorkflowType);

        IWorkflow workflow = engine.Find(item.EntityId, workflowDefinition.EntityType);
        TriggerParam triggerParam = new TriggerParam(item.TriggerName, workflow);

        return await engine.TriggerAsync(triggerParam);
      }
    }
  }
}