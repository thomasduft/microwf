using System.Linq;
using tomware.Microwf.Domain;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.AspNetCoreEngine
{
  public class WorkflowTest
  {
    [Fact]
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
      Assert.NotNull(workflow);
      Assert.Equal(workflow.CorrelationId, correlationId);
      Assert.Equal(workflow.Type, type);
      Assert.Equal(workflow.State, state);
      Assert.Equal(workflow.Assignee, assignee);
    }

    [Fact]
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
      Assert.NotNull(workflow);
      Assert.Single(workflow.WorkflowVariables);
    }

    [Fact]
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
      Assert.NotNull(workflow);
      Assert.Single(workflow.WorkflowVariables);

      var wv = workflow.WorkflowVariables.First();
      var v = (LightSwitcherWorkflowVariable)WorkflowVariable.ConvertContent(wv);
      Assert.True(v.CanSwitch);
    }
  }
}