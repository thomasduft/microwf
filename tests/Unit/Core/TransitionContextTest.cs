using System;
using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class TransitionContextTest
  {
    [Fact]
    public void TransitionContext_NewInstance_CreatesANewInstance()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };

      // Act
      var context = new TransitionContext(switcher);

      // Assert
      Assert.NotNull(context);
      Assert.Equal(context.Instance, switcher);
      Assert.False(context.TransitionAborted);
      Assert.False(context.HasVariables);
      Assert.False(context.HasErrors);
    }

    [Fact]
    public void TransitionContext_AddError_TransitionHasErrors()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);
      var error = "Some error";

      // Act
      context.AddError(error);

      // Assert
      Assert.NotNull(context);
      Assert.False(context.HasVariables);
      Assert.True(context.HasErrors);
    }

    [Fact]
    public void TransitionContext_AbortTransition_TransitionIsAbortedAndHasErrors()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);
      var reason = "Some good reason in order to abort the transiton";

      // Act
      context.AbortTransition(reason);

      // Assert
      Assert.NotNull(context);
      Assert.True(context.TransitionAborted);
      Assert.False(context.HasVariables);
      Assert.True(context.HasErrors);
    }

    [Fact]
    public void TransitionContext_GetInstance_InstanceReturned()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);

      // Act
      var worflow = context.GetInstance<Switcher>();

      // Assert
      Assert.NotNull(worflow);
      Assert.Equal(worflow, switcher);
      Assert.Equal(worflow.State, switcher.State);
    }

    [Fact]
    public void TransitionContext_SetVariable_VariableIsSet()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);
      var variable = new SwitcherWorkflowVariable(true);

      // Act
      context.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
        variable);

      // Assert
      Assert.NotNull(context);
      Assert.True(context.HasVariables);
      Assert.True(context.ContainsKey(SwitcherWorkflowVariable.KEY));
    }

    [Fact]
    public void TransitionContext_SetVariable_ReplacesExistingVariableValue()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);
      var variable = new SwitcherWorkflowVariable(true);

      context.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
        variable);

      Assert.True(context
        .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY)
        .CanSwitch);

      variable.CanSwitch = false;

      // Act
      context.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
        variable);

      // Assert
      Assert.NotNull(context);
      Assert.True(context.HasVariables);
      Assert.True(context.ContainsKey(SwitcherWorkflowVariable.KEY));
      Assert.False(context
        .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY)
        .CanSwitch);
    }

    [Fact]
    public void TransitionContext_GetVariable_VariableReturned()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);
      var canSwitch = new SwitcherWorkflowVariable(true);

      context.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
        canSwitch);

      // Act
      var variable = context
        .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY);

      // Assert
      Assert.NotNull(variable);
      Assert.True(variable.CanSwitch);
    }

    [Fact]
    public void TransitionContext_GetVariable_VariableNotPresent()
    {
      // Arrange
      Switcher switcher = new Switcher
      {
        Type = OnOffWorkflow.TYPE
      };
      var context = new TransitionContext(switcher);

      // Act
      Assert.Throws<Exception>(
        () => context.GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY));
    }
  }
}