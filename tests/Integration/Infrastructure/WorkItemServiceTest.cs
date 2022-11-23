using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.Tests.Integration.Utils;
using Xunit;

namespace tomware.Microwf.Tests.Integration.Infrastructure
{
  public class WorkItemServiceTest
  {
    [Fact]
    public async Task WorkItemService_GetUpCommingsAsync_UpCommingsReturned()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();

      var dueDate = SystemTime.Now().AddMinutes(2);

      var workItems = this.GetWorkItems(dueDate);
      await context.WorkItems.AddRangeAsync(workItems);
      await context.SaveChangesAsync();

      var parameters = new PagingParameters();

      // Act
      var upcommings = await service.GetUpCommingsAsync(parameters);

      // Assert
      Assert.NotNull(upcommings);
      Assert.Equal(2, upcommings.Count);
    }

    [Fact]
    public async Task WorkItemService_GetFailedAsync_FailedReturned()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();

      var workItems = this.GetWorkItems();
      workItems.First().Retries = 4;
      await context.WorkItems.AddRangeAsync(workItems);
      await context.SaveChangesAsync();

      var parameters = new PagingParameters();

      // Act
      var failed = await service.GetFailedAsync(parameters);

      // Assert
      Assert.NotNull(failed);
      Assert.Single(failed);
      Assert.Equal(1, failed.First().Id);
      Assert.Equal(4, failed.First().Retries);
    }

    [Fact]
    public async Task WorkItemService_ResumeWorkItemsAsync_TwoItemsResumed()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      await context.WorkItems.AddRangeAsync(this.GetWorkItems());
      await context.SaveChangesAsync();

      var service = serviceProvider.GetRequiredService<IWorkItemService>();

      // Act
      IEnumerable<WorkItem> resumedItems = await service.ResumeWorkItemsAsync();

      // Assert
      Assert.Equal(2, resumedItems.Count());
    }

    [Fact]
    public async Task WorkItemService_PersistWorkItemsAsync_TwoItemsPersisted()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();
      var workItems = this.GetWorkItems();

      // Act
      await service.PersistWorkItemsAsync(workItems);

      // Assert
      Assert.Equal(2, context.WorkItems.Count());
    }

    [Fact]
    public async Task WorkItemService_PersistWorkItemsAsync_OneItemPersistedOneItemUpdated()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();

      var workItems = this.GetWorkItems();

      var firstWorkItem = workItems.First();
      firstWorkItem.WorkflowType = "firstCopy";

      await context.WorkItems.AddAsync(firstWorkItem);
      await context.SaveChangesAsync();

      firstWorkItem.WorkflowType = "first";

      // Act
      await service.PersistWorkItemsAsync(workItems);

      // Assert
      Assert.Equal(2, context.WorkItems.Count());
      Assert.Equal("first", context.WorkItems.First().WorkflowType);
    }

    [Fact]
    public async Task WorkItemService_Reschedule_WorkItemRescheduled()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();
      var repository = serviceProvider.GetRequiredService<IWorkItemRepository>();

      var workItems = this.GetWorkItems();
      await context.WorkItems.AddRangeAsync(workItems);
      await context.SaveChangesAsync();


      var dueDate = SystemTime.Now().AddMinutes(1);
      var model = new tomware.Microwf.Infrastructure.WorkItemDto
      {
        Id = 1,
        DueDate = dueDate
      };

      // Act
      await service.Reschedule(model);

      // Assert
      var rescheduledItem = await repository.GetByIdAsync(1);
      Assert.NotNull(rescheduledItem);
      Assert.Equal(rescheduledItem.DueDate, dueDate);
    }

    [Fact]
    public async Task WorkItemService_DeleteAsync_OneItemDeleted()
    {
      // Arrange
      var diHelper = new DITestHelper();
      var serviceProvider = diHelper.BuildDefault();

      var context = serviceProvider.GetRequiredService<TestDbContext>();
      var service = serviceProvider.GetRequiredService<IWorkItemService>();

      var workItems = this.GetWorkItems();
      await context.WorkItems.AddRangeAsync(workItems);
      await context.SaveChangesAsync();

      // Act
      await service.DeleteAsync(1);

      // Assert
      Assert.Equal(1, context.WorkItems.Count());
      Assert.Equal(2, context.WorkItems.First().Id);
    }

    private List<WorkItem> GetWorkItems(DateTime? dueDate = null)
    {
      List<WorkItem> workItems = new List<WorkItem>() {
        new WorkItem {
          Id = 1,
          WorkflowType = "first",
          TriggerName = "triggerFirst",
          EntityId = 1,
          DueDate = dueDate ?? SystemTime.Now()
        },
        new WorkItem {
          Id = 2,
          WorkflowType = "second",
          TriggerName = "triggerSecond",
          EntityId = 1,
          DueDate = dueDate ?? SystemTime.Now()
        }
      };

      return workItems;
    }
  }
}