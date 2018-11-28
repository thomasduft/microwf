using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IJobQueueService
  {
    /// <summary>
    /// Indicates whether the queue still has items to process.
    /// </summary>
    /// <param name="workItem"></param>
    bool HasItemsInQueue { get; }

    /// <summary>
    /// Enqueues a work item.
    /// </summary>
    /// <param name="workItem"></param>
    void Enqueue(WorkItem workItem);

    /// <summary>
    /// Dequeues a work item.
    /// </summary>
    /// <returns></returns>
    WorkItem Dequeue();

    /// <summary>
    /// Processes work items.
    /// </summary>
    /// <returns></returns>
    Task ProcessItemsAsync();

    /// <summary>
    /// Resumes persisted work items.
    /// </summary>
    void ResumeWorkItems();

    /// <summary>
    /// Persists running work items.
    /// </summary>
    /// <returns></returns>
    Task PersistWorkItemsAsync();
  }

  public class JobQueueService : IJobQueueService
  {
    private readonly ILogger<JobQueueService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private ConcurrentQueue<WorkItem> _items;

    private ConcurrentQueue<WorkItem> Items
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

    public bool HasItemsInQueue
    {
      get
      {
        return this.Items.Count > 0;
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

    public async Task ProcessItemsAsync()
    {
      while (Items.Count > 0)
      {
        var item = Dequeue();
        if (item == null) continue;

        try
        {
          _logger.LogTrace($"Processing work item {item.ToString()}");

          TriggerResult triggerResult = await ProcessItemAsync(item);
          await this.HandleTriggerResult(triggerResult, item);
        }
        catch (Exception ex)
        {
          _logger.LogError("Processing work item failed", ex, item);
          item.Error = $"{ex.Message} - {ex.StackTrace}";
          item.Retries++;

          Enqueue(item);
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
      _logger.LogTrace("Deleting work item", item);

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
        var engine = serviceProvider.GetRequiredService<IWorkflowEngine>();
        var workflowDefinitionProvider
          = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

        EntityWorkflowDefinitionBase workflowDefinition
         = (EntityWorkflowDefinitionBase)workflowDefinitionProvider
                                           .GetWorkflowDefinition(item.WorkflowType);

        IWorkflow workflow = engine.Find(item.EntityId, workflowDefinition.EntityType);
        TriggerParam triggerParam = new TriggerParam(item.TriggerName, workflow);

        return await engine.TriggerAsync(triggerParam);
      }
    }

    private async Task HandleTriggerResult(TriggerResult triggerResult, WorkItem item)
    {
      if (triggerResult.HasErrors || triggerResult.IsAborted)
      {
        item.Error = string.Join(" - ", triggerResult.Errors);
        _logger.LogError("HandleTriggerResult", item.Error, triggerResult);

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
  }
}