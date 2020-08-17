using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class TriggerResultTest
  {
    [Fact]
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
      Assert.NotNull(result);
      Assert.True(result.CanTrigger);
      Assert.False(result.IsAborted);
      Assert.Same(result.TriggerName, trigger);
      Assert.False(result.HasErrors);
      Assert.Same(result.CurrentState, switcher.State);
      Assert.False(result.HasAutoTrigger);
    }

    [Fact]
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
      Assert.NotNull(result);
      Assert.True(result.CanTrigger);
      Assert.False(result.IsAborted);
      Assert.Same(result.TriggerName, trigger);
      Assert.False(result.HasErrors);
      Assert.Same(result.CurrentState, switcher.State);
      Assert.False(result.HasAutoTrigger);

      var v = result.GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY);
      Assert.NotNull(v);
      Assert.True(v.CanSwitch);
    }
  }
}