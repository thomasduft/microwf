using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Domain;
using Xunit;

namespace microwf.Tests.AspNetCoreEngine
{
  public class WorkItemTest
  {
    [Fact]
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
      Assert.NotNull(workItem);
      Assert.Equal(workItem.TriggerName, triggerName);
      Assert.Equal(workItem.EntityId, entityId);
      Assert.Equal(workItem.WorkflowType, workflowType);
      Assert.Equal(workItem.DueDate, dueDate);
      Assert.Equal(0, workItem.Retries);
    }
  }
}