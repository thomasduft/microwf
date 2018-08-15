using Microsoft.VisualStudio.TestTools.UnitTesting;
using tomware.Microwf.Core;
using microwf.Tests.WorkflowDefinitions;
using System.Collections.Generic;
using System.Linq;

namespace microwf.Tests.Core
{
  [TestClass]
  public class WorkflowExecutionTest
  {
    [TestMethod]
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
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count());
      Assert.AreEqual("SwitchOn", result.First().TriggerName);
    }

    [TestMethod]
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
      Assert.IsNotNull(result);
      Assert.AreEqual(true, result.CanTrigger);
    }

    [TestMethod]
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
      Assert.IsNotNull(result);
      Assert.AreEqual(false, result.CanTrigger);
    }

    [TestMethod]
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
      Assert.IsNotNull(switcher);
      Assert.AreEqual("On", result.CurrentState);

      Assert.IsNotNull(result);
      Assert.AreEqual("SwitchOn", result.TriggerName);
    }
  }
}
