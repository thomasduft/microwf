using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Domain;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkItemTest
  {
    [TestMethod]
    public void WorkItem_Create_NewInstanceCreated()
    {
      // Arrange
      var triggerName = "SwitchOn";
      var entityId = 1;
      var workflowType = EntityOnOffWorkflow.TYPE;
      var dueDate = SystemTime.Now();

      // Act
      var workItem = WorkItem.Create(triggerName, entityId, workflowType, dueDate);

      // Assert
      Assert.IsNotNull(workItem);
      Assert.AreEqual(workItem.TriggerName, triggerName);
      Assert.AreEqual(workItem.EntityId, entityId);
      Assert.AreEqual(workItem.WorkflowType, workflowType);
      Assert.AreEqual(workItem.DueDate, dueDate);
      Assert.AreEqual(workItem.Retries, 0);
    }
  }
}