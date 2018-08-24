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
    private ConcurrentQueue<WorkItem> _items;

    public ConcurrentQueue<WorkItem> Items
    {
      get
      {
        if (_items == null)
        {
          _items = new ConcurrentQueue<WorkItem>();
          ResumeWorkItems();
        }

        return _items;
      }
    }

    public JobQueueService(
      ILogger<JobQueueService> logger,
      IServiceScopeFactory serviceScopeFactory
    )
    {
      _logger = logger;
      _serviceScopeFactory = serviceScopeFactory;
    }

    public void Enqueue(WorkItem item)
    {
      _logger.LogTrace("Enqueue work item", item);
      if (item.Retries > 3)
      {
        _logger.LogInformation($"Amount of retries for work item ${item.Id} exceeded");
        return;
      }

      Items.Enqueue(item);
    }

    public WorkItem Dequeue()
    {
      _logger.LogTrace("Dequeued work item");

      if (Items.TryDequeue(out WorkItem item))
      {
        item.Error = string.Empty;

        return item;
      }

      return null;
    }

    public async Task TriggerAsync()
    {
      _logger.LogTrace("Triggering job queue for doing work");

      while (Items.Count > 0)
      {
        var item = Dequeue();
        if (item == null) continue;

        TriggerResult triggerResult = await ProcessItemAsync(item);
        if (triggerResult.HasErrors || triggerResult.IsAborted)
        {
          _logger.LogError(
            "ProcessingWorkItemFailed",
            string.Join("-", triggerResult.Errors),
            triggerResult
          );

          item.Retries++;
          Enqueue(item);
        }
        else
        {
          if (item.Id > 0)
          {
            // Delete it from db if it was once persisted!
            await DeleteWorkItem(item);
          }
        }
      }

      await Task.CompletedTask;
    }

    public void ResumeWorkItems()
    {
      _logger.LogTrace("Resuming work items");

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        var items = service.ResumeWorkItemsAsync()
          .GetAwaiter()
          .GetResult();

        foreach (var item in items)
        {
          Enqueue(item);
        }
      }
    }

    public async Task PersistWorkItemsAsync()
    {
      _logger.LogTrace("Persisting work items");

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        await service.PersistWorkItemsAsync(Items.ToArray());
      }
    }

    public async Task DeleteWorkItem(WorkItem item)
    {
      _logger.LogTrace("Deleting work item");

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        await service.DeleteAsync(item.Id);
      }
    }

    private async Task<TriggerResult> ProcessItemAsync(WorkItem item)
    {
      _logger.LogTrace("Processing work item", item);

      using (var scope = _serviceScopeFactory.CreateScope())
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