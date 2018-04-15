using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.tests.WorkflowDefinitions;
using tomware.Microwf.Core;

namespace microwf.tests
{
  [TestClass]
  public class TriggerResultTest
  {
    [TestMethod]
    public void TriggerResult_NewInstance_CreatesANewInstance()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var transitionContext = new TransitionContext(switcher);
      var canTrigger = true;

      // Act
      var result = new TriggerResult(trigger, transitionContext, canTrigger);

      // Assert
      Assert.IsNotNull(result);
      Assert.IsTrue(result.CanTrigger);
      Assert.IsFalse(result.IsAborted);
      Assert.AreSame(result.TriggerName, trigger);
      Assert.IsFalse(result.HasErrors);
      Assert.AreSame(result.CurrentState, switcher.State);
    }

    [TestMethod]
    public void TriggerResult_NewInstanceWithVariables_CreatesANewInstance()
    {
      // Arrange
      var trigger = "SwitchOn";
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var transitionContext = new TransitionContext(switcher);
      var variable = new SwitcherWorkflowVariable(true);
      transitionContext.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
         variable);
      var canTrigger = true;

      // Act
      var result = new TriggerResult(trigger, transitionContext, canTrigger);

      // Assert
      Assert.IsNotNull(result);
      Assert.IsTrue(result.CanTrigger);
      Assert.IsFalse(result.IsAborted);
      Assert.AreSame(result.TriggerName, trigger);
      Assert.IsFalse(result.HasErrors);
      Assert.AreSame(result.CurrentState, switcher.State);

      var v = result.GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY);
      Assert.IsNotNull(v);
      Assert.IsTrue(v.CanSwitch);
    }
  }
}