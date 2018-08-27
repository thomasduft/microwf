using Microsoft.VisualStudio.TestTools.UnitTesting;
using tomware.Microwf.Core;
using microwf.Tests.WorkflowDefinitions;
using System.Collections.Generic;
using System.Linq;

namespace microwf.Tests.Core
{
  [TestClass]
  public class WorkflowDefinitionTest
  {
    [TestMethod]
    public void States_HolidayApprovalWorkflow_Returns4States()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<string> states = workflow.States;

      // Assert
      Assert.IsNotNull(states);
      Assert.AreEqual(4, states.Count());
    }

    [TestMethod]
    public void Triggers_HolidayApprovalWorkflow_Returns3Triggers()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<string> triggers = workflow.Triggers;

      // Assert
      Assert.IsNotNull(triggers);
      Assert.AreEqual(3, triggers.Count());
    }

    [TestMethod]
    public void Transitions_HolidayApprovalWorkflow_Returns3Transitions()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<Transition> transitions = workflow.Transitions;

      // Assert
      Assert.IsNotNull(transitions);
      Assert.AreEqual(3, transitions.Count());
    }
  }
}
