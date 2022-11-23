using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class WorkflowExecutionTest
  {
    [Fact]
    public void GetTriggers_InitialStateIsOff_TriggerNameIsSwitchOn()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      WorkflowExecution execution = new WorkflowExecution(new OnOffWorkflow());

      // Act
      IEnumerable<TriggerResult> result = execution.GetTriggers(switcher);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Equal("SwitchOn", result.First().TriggerName);
    }

    [Fact]
    public void CanTrigger_InitialStateIsOff_CanTriggerToStateOn()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      WorkflowExecution execution = new WorkflowExecution(new OnOffWorkflow());

      // Act
      TriggerResult result = execution.CanTrigger(new TriggerParam("SwitchOn", switcher));

      // Assert
      Assert.NotNull(result);
      Assert.True(result.CanTrigger);
    }

    [Fact]
    public void CanTrigger_InitialStateIsOff_CanNotTriggerToStateOn()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      WorkflowExecution execution = new WorkflowExecution(new OnOffWorkflow());

      var variables = new Dictionary<string, WorkflowVariableBase>();
      var variable = new SwitcherWorkflowVariable(false);
      variables.Add(SwitcherWorkflowVariable.KEY, variable);
      var triggerParam = new TriggerParam("SwitchOn", switcher, variables);

      // Act
      TriggerResult result = execution.CanTrigger(triggerParam);

      // Assert
      Assert.NotNull(result);
      Assert.False(result.CanTrigger);
    }

    [Fact]
    public void Trigger_InitialStateIsOff_StateIsOn()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      WorkflowExecution execution = new WorkflowExecution(new OnOffWorkflow());

      // Act
      TriggerResult result = execution.Trigger(new TriggerParam("SwitchOn", switcher));

      // Assert
      Assert.NotNull(switcher);
      Assert.Equal("On", result.CurrentState);

      Assert.NotNull(result);
      Assert.Equal("SwitchOn", result.TriggerName);
      Assert.True(result.HasAutoTrigger);
      Assert.Equal("SwitchOff", result.AutoTrigger.Trigger);
      Assert.True(result.AutoTrigger.DueDate.HasValue);
    }

    [Fact]
    public void Trigger_InitialStateIsOn_StateIsOff()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE,
        State = "On"
      };
      WorkflowExecution execution = new WorkflowExecution(new OnOffWorkflow());

      // Act
      TriggerResult result = execution.Trigger(new TriggerParam("SwitchOff", switcher));

      // Assert
      Assert.NotNull(switcher);
      Assert.Equal("Off", result.CurrentState);

      Assert.NotNull(result);
      Assert.Equal("SwitchOff", result.TriggerName);
      Assert.False(result.HasAutoTrigger);
    }
  }
}