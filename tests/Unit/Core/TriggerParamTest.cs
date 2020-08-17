using System;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class TriggerParamTest
  {
    [Fact]
    public void TriggerParam_NewInstance_CreatesANewInstance()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      // Act
      var triggerParam = new TriggerParam(trigger, switcher);

      // Assert
      Assert.NotNull(triggerParam);
      Assert.Equal(triggerParam.TriggerName, trigger);
      Assert.Equal(triggerParam.Instance, switcher);
      Assert.False(triggerParam.HasVariables);
      Assert.NotNull(triggerParam.Variables);
    }

    [Fact]
    public void TriggerParam_NewInstanceWithVariables_CreatesANewInstance()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var variables = new Dictionary<string, WorkflowVariableBase>();
      var variable = new SwitcherWorkflowVariable(true);
      variables.Add(SwitcherWorkflowVariable.KEY, variable);

      // Act
      var triggerParam = new TriggerParam(trigger, switcher, variables);

      // Assert
      Assert.NotNull(triggerParam);
      Assert.Equal(triggerParam.TriggerName, trigger);
      Assert.Equal(triggerParam.Instance, switcher);
      Assert.True(triggerParam.HasVariables);
      Assert.NotNull(triggerParam.Variables);
    }

    [Fact]
    public void TriggerParam_NewInstanceWithFluentVariables_CreatesANewInstance()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      // Act
      var triggerParam = new TriggerParam(trigger, switcher)
        .AddVariable(SwitcherWorkflowVariable.KEY, new SwitcherWorkflowVariable(true));

      // Assert
      Assert.NotNull(triggerParam);
      Assert.Equal(triggerParam.TriggerName, trigger);
      Assert.Equal(triggerParam.Instance, switcher);
      Assert.True(triggerParam.HasVariables);
      Assert.NotNull(triggerParam.Variables);
    }

    [Fact]
    public void TriggerParam_AddsTheSameWorkflowVariable_ThrowsException()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      // Act
      Assert.Throws<InvalidOperationException>(
        () => new TriggerParam(trigger, switcher)
        .AddVariable(SwitcherWorkflowVariable.KEY, new SwitcherWorkflowVariable(true))
        .AddVariable(SwitcherWorkflowVariable.KEY, new SwitcherWorkflowVariable(true)));
    }
  }
}