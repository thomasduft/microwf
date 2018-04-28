using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.tests.WorkflowDefinitions;
using tomware.Microwf.Core;

namespace microwf.tests
{
  [TestClass]
  public class TriggerParamTest
  {
    [TestMethod]
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
      Assert.IsNotNull(triggerParam);
      Assert.AreEqual(triggerParam.TriggerName, trigger);
      Assert.AreEqual(triggerParam.Instance, switcher);
      Assert.IsFalse(triggerParam.HasVariables);
      Assert.IsNotNull(triggerParam.Variables);
    }

    [TestMethod]
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
      Assert.IsNotNull(triggerParam);
      Assert.AreEqual(triggerParam.TriggerName, trigger);
      Assert.AreEqual(triggerParam.Instance, switcher);
      Assert.IsTrue(triggerParam.HasVariables);
      Assert.IsNotNull(triggerParam.Variables);
    }

    [TestMethod]
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
      Assert.IsNotNull(triggerParam);
      Assert.AreEqual(triggerParam.TriggerName, trigger);
      Assert.AreEqual(triggerParam.Instance, switcher);
      Assert.IsTrue(triggerParam.HasVariables);
      Assert.IsNotNull(triggerParam.Variables);
    }

    [TestMethod]
    public void TriggerParam_AddsTheSameWorkflowVariable_ThrowsException()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      // Act
      Assert.ThrowsException<InvalidOperationException>(
        () => new TriggerParam(trigger, switcher)
        .AddVariable(SwitcherWorkflowVariable.KEY, new SwitcherWorkflowVariable(true))
        .AddVariable(SwitcherWorkflowVariable.KEY, new SwitcherWorkflowVariable(true)));
    }
  }
}