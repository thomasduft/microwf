using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkItemServiceTest
  {
    [TestMethod]
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
      Assert.IsNotNull(upcommings);
      Assert.AreEqual(upcommings.Count, 2);
    }

    [TestMethod]
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
      Assert.IsNotNull(failed);
      Assert.AreEqual(failed.Count, 1);
      Assert.AreEqual(failed.First().Id, 1);
      Assert.AreEqual(failed.First().Retries, 4);
    }

    [TestMethod]
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
      Assert.AreEqual(2, resumedItems.Count());
    }

    [TestMethod]
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
      Assert.AreEqual(2, context.WorkItems.Count());
    }

    [TestMethod]
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
      Assert.AreEqual(2, context.WorkItems.Count());
      Assert.AreEqual("first", context.WorkItems.First().WorkflowType);
    }

    [TestMethod]
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
      Assert.IsNotNull(rescheduledItem);
      Assert.AreEqual(rescheduledItem.DueDate, dueDate);
    }

    [TestMethod]
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
      Assert.AreEqual(1, context.WorkItems.Count());
      Assert.AreEqual(2, context.WorkItems.First().Id);
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