using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IJobQueueService
  {
    /// <summary>
    /// Enqueues a work item.
    /// </summary>
    /// <param name="workItem"></param>
    Task Enqueue(WorkItem workItem);

    /// <summary>
    /// Processes work items.
    /// </summary>
    /// <returns></returns>
    Task ProcessItemsAsync();

    /// <summary>
    /// Resumes persisted work items.
    /// </summary>
    Task ResumeWorkItems();

    /// <summary>
    /// Persists running work items.
    /// </summary>
    /// <returns></returns>
    Task PersistWorkItemsAsync();

    /// <summary>
    /// Returns a snapshot of the current queued work items.
    /// </summary>
    /// <returns></returns>
    IEnumerable<WorkItem> GetSnapshot();
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

    public async Task Enqueue(WorkItem item)
    {
      _logger.LogTrace("Enqueue work item", item);

      if (item.Retries > Constants.WORKITEM_RETRIES)
      {
        _logger.LogInformation(
          "Amount of retries for work item {WorkItem} exceeded!",
          LogHelper.SerializeObject(item)
        );

        await this.PersistWorkItemsAsync(new List<WorkItem> { item });
      }
      else
      {
        Items.Enqueue(item);

        await Task.CompletedTask;
      }
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

          await Enqueue(item);
        }
      }

      await Task.CompletedTask;
    }

    public async Task ResumeWorkItems()
    {
      _logger.LogTrace("Resuming work items");

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        var items = await service.ResumeWorkItemsAsync();

        foreach (var item in items)
        {
          await Enqueue(item);
        }
      }
    }

    public async Task PersistWorkItemsAsync()
    {
      _logger.LogTrace("Persisting work items");

      await this.PersistWorkItemsAsync(Items.ToArray());
    }

    public IEnumerable<WorkItem> GetSnapshot()
    {
      return this.Items.ToArray();
    }

    private WorkItem Dequeue()
    {
      if (Items.TryDequeue(out WorkItem item))
      {
        item.Error = string.Empty;

        return item;
      }

      return null;
    }

    private async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      _logger.LogTrace("Persisting work items");

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        await service.PersistWorkItemsAsync(items);
      }
    }

    private async Task DeleteWorkItem(WorkItem item)
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
        _logger.LogError(
          "Bad TriggerResult: {TriggerResult}",
          LogHelper.SerializeObject(triggerResult)
        );

        item.Retries++;
        await Enqueue(item);
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