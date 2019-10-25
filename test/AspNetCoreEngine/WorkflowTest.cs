using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.WorkflowDefinitions;
using System.Linq;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkflowTest
  {
    [TestMethod]
    public void Workflow_Create_NewInstanceCreated()
    {
      // Arrange
      var correlationId = 100;
      var type = EntityOnOffWorkflow.TYPE;
      var state = "Off";
      var assignee = "alice";

      // Act
      var workflow = Workflow.Create(correlationId, type, state, assignee);

      // Assert
      Assert.IsNotNull(workflow);
      Assert.AreEqual(workflow.CorrelationId, correlationId);
      Assert.AreEqual(workflow.Type, type);
      Assert.AreEqual(workflow.State, state);
      Assert.AreEqual(workflow.Assignee, assignee);
    }

    [TestMethod]
    public void Workflow_AddVariable_VariableAdded()
    {
      // Arrange
      var correlationId = 100;
      var type = EntityOnOffWorkflow.TYPE;
      var state = "Off";
      var assignee = "alice";
      var workflow = Workflow.Create(correlationId, type, state, assignee);

      var variable = new LightSwitcherWorkflowVariable();

      // Act
      workflow.AddVariable(variable);

      // Assert
      Assert.IsNotNull(workflow);
      Assert.AreEqual(workflow.WorkflowVariables.Count, 1);
    }

    [TestMethod]
    public void Workflow_AddExistingVariable_VariableAdded()
    {
      // Arrange
      var correlationId = 100;
      var type = EntityOnOffWorkflow.TYPE;
      var state = "Off";
      var assignee = "alice";
      var workflow = Workflow.Create(correlationId, type, state, assignee);

      var variable = new LightSwitcherWorkflowVariable();
      workflow.AddVariable(variable);

      var existingVariable = new LightSwitcherWorkflowVariable
      {
        CanSwitch = true
      };

      // Act
      workflow.AddVariable(existingVariable);

      // Assert
      Assert.IsNotNull(workflow);
      Assert.AreEqual(workflow.WorkflowVariables.Count, 1);

      var wv = workflow.WorkflowVariables.First();
      var v = (LightSwitcherWorkflowVariable)WorkflowVariable.ConvertContent(wv);
      Assert.IsTrue(v.CanSwitch);
    }
  }
}