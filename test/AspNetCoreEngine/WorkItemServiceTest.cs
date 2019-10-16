using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkItemServiceTest
  {
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

    private List<WorkItem> GetWorkItems()
    {
      List<WorkItem> workItems = new List<WorkItem>() {
        new WorkItem {
          Id = 1,
          WorkflowType = "first",
          TriggerName = "triggerFirst",
          EntityId = 1
        },
        new WorkItem {
          Id = 2,
          WorkflowType = "second",
          TriggerName = "triggerSecond",
          EntityId = 1
        }
      };

      return workItems;
    }
  }
}