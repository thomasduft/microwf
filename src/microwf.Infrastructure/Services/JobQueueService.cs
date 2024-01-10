using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class JobQueueService : IJobQueueService
  {
    private readonly ILogger<JobQueueService> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private ConcurrentQueue<WorkItem> items;

    private ConcurrentQueue<WorkItem> Items
    {
      get
      {
        return this.items ??= new ConcurrentQueue<WorkItem>();
      }
    }

    public JobQueueService(
      ILogger<JobQueueService> logger,
      IServiceScopeFactory serviceScopeFactory
    )
    {
      this.logger = logger;
      this.serviceScopeFactory = serviceScopeFactory;
    }

    public JobQueueService(ILogger<JobQueueService> logger)
    {
      this.logger = logger;
    }

    public async Task Enqueue(WorkItem item)
    {
      this.logger.LogTrace("Enqueue work item {@WorkItem}", item);

      if (item.Retries > WorkItem.WORKITEM_RETRIES)
      {
        this.logger.LogInformation("Amount of retries for work item {@WorkItem} exceeded!", item);

        await this.PersistWorkItemsAsync(new List<WorkItem> { item });
      }
      else
      {
        this.Items.Enqueue(item);

        await Task.CompletedTask;
      }
    }

    public async Task ProcessItemsAsync()
    {
      while (!this.Items.IsEmpty)
      {
        var item = this.Dequeue();
        if (item == null) continue;

        try
        {
          this.logger.LogTrace(
            "Processing work item {@WorkItem}",
            item
          );

          await this.ProcessItemInternal(item);
        }
        catch (Exception ex)
        {
          this.logger.LogError(
            ex,
            "Processing of work item {@WorkItem} failed",
            item
          );
          item.Error = $"{ex.Message} - {ex.StackTrace}";
          item.Retries++;

          await this.Enqueue(item);
        }
      }

      await Task.CompletedTask;
    }

    public async Task ResumeWorkItems()
    {
      this.logger.LogTrace("Resuming work items");

      using (var scope = this.serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        var items = await service.ResumeWorkItemsAsync();

        foreach (var item in items)
        {
          await this.Enqueue(item);
        }
      }
    }

    public async Task PersistWorkItemsAsync()
    {
      this.logger.LogTrace("Persisting work items");

      await this.PersistWorkItemsAsync(this.Items.ToArray());
    }

    public IEnumerable<Domain.WorkItemDto> GetSnapshot()
    {
      return ObjectMapper.ToWorkItemViewModelList(this.Items.ToArray());
    }

    private WorkItem Dequeue()
    {
      if (this.Items.TryDequeue(out WorkItem item))
      {
        item.Error = string.Empty;

        return item;
      }

      return null;
    }

    private async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
    {
      this.logger.LogTrace("Persisting work items");

      using (var scope = this.serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        await service.PersistWorkItemsAsync(items);
      }
    }

    private async Task DeleteWorkItem(WorkItem item)
    {
      this.logger.LogTrace("Deleting work item {WorkItem}", item);

      using (var scope = this.serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var service = serviceProvider.GetRequiredService<IWorkItemService>();
        await service.DeleteAsync(item.Id);
      }
    }

    private async Task ProcessItemInternal(WorkItem item)
    {
      TriggerResult triggerResult = await this.ProcessItemAsync(item);
      await this.HandleTriggerResult(triggerResult, item);
    }

    private async Task<TriggerResult> ProcessItemAsync(WorkItem item)
    {
      this.logger.LogTrace("Processing work item {WorkItem}", item);

      using (var scope = this.serviceScopeFactory.CreateScope())
      {
        IServiceProvider serviceProvider = scope.ServiceProvider;
        var engine = serviceProvider.GetRequiredService<IWorkflowEngineService>();
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
        this.logger.LogError(
          "Bad TriggerResult: {@TriggerResult}",
          triggerResult
        );

        item.Retries++;
        await this.Enqueue(item);
      }
      else
      {
        if (item.Id > 0)
        {
          // Delete it from db if it was once persisted!
          await this.DeleteWorkItem(item);
        }
      }
    }
  }
}