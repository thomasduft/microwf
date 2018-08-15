using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class TriggerParamTest
  {
    [TestMethod]
    public async Task WorkItemService_ResumeWorkItemsAsync_TwoItemsResumed()
    {
      // Arrange
      var options = TestDbContext.CreateDbContextOptions();
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

      var context = new EngineDbContext(options);
      await context.WorkItems.AddRangeAsync(workItems);
      await context.SaveChangesAsync();

      var diHelper = new DITestHelper();
      var loggerFactory = diHelper.GetLoggerFactory();
      ILogger<WorkItemService<EngineDbContext>> logger = loggerFactory
        .CreateLogger<WorkItemService<EngineDbContext>>();

      var service = new WorkItemService<EngineDbContext>(context, logger);

      // Act
      IEnumerable<WorkItem> resumedItems = await service.ResumeWorkItemsAsync();

      // Assert
      Assert.AreEqual(2, resumedItems.Count());
    }

        [TestMethod]
    public async Task WorkItemService_PersistWorkItemsAsync_TwoItemsPersisted()
    {
      // Arrange
      var options = TestDbContext.CreateDbContextOptions();
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

      var context = new EngineDbContext(options);
      var diHelper = new DITestHelper();
      var loggerFactory = diHelper.GetLoggerFactory();
      ILogger<WorkItemService<EngineDbContext>> logger = loggerFactory
        .CreateLogger<WorkItemService<EngineDbContext>>();

      var service = new WorkItemService<EngineDbContext>(context, logger);

      // Act
      await service.PersistWorkItemsAsync(workItems);

      // Assert
      Assert.AreEqual(2, context.WorkItems.Count());
    }
  }
}