using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.tests.WorkflowDefinitions;
using tomware.Microwf.Core;

namespace microwf.tests
{
  [TestClass]
  public class TransitionContextTest
  {
    [TestMethod]
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
      Assert.IsNotNull(context);
      Assert.AreEqual(context.Instance, switcher);
      Assert.IsFalse(context.TransitionAborted);
      Assert.IsFalse(context.HasVariables);
      Assert.IsFalse(context.HasErrors);
    }

    [TestMethod]
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
      Assert.IsNotNull(context);
      Assert.IsFalse(context.HasVariables);
      Assert.IsTrue(context.HasErrors);
    }

    [TestMethod]
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
      Assert.IsNotNull(context);
      Assert.IsTrue(context.TransitionAborted);
      Assert.IsFalse(context.HasVariables);
      Assert.IsTrue(context.HasErrors);
    }

    [TestMethod]
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
      Assert.IsNotNull(worflow);
      Assert.AreEqual(worflow, switcher);
      Assert.AreEqual(worflow.State, switcher.State);
    }

    [TestMethod]
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
      Assert.IsNotNull(context);
      Assert.IsTrue(context.HasVariables);
      Assert.IsTrue(context.ContainsKey(SwitcherWorkflowVariable.KEY));
    }

    [TestMethod]
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

      Assert.IsTrue(context
        .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY)
        .CanSwitch);

      variable.CanSwitch = false;

      // Act
      context.SetVariable<SwitcherWorkflowVariable>(
        SwitcherWorkflowVariable.KEY,
        variable);

      // Assert
      Assert.IsNotNull(context);
      Assert.IsTrue(context.HasVariables);
      Assert.IsTrue(context.ContainsKey(SwitcherWorkflowVariable.KEY));
      Assert.IsFalse(context
        .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY)
        .CanSwitch);
    }

    [TestMethod]
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
      Assert.IsNotNull(variable);
      Assert.IsTrue(variable.CanSwitch);
    }
  }
}