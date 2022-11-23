using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class WorkflowDefinitionTest
  {
    [Fact]
    public void States_HolidayApprovalWorkflow_Returns4States()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<string> states = workflow.States;

      // Assert
      Assert.NotNull(states);
      Assert.Equal(4, states.Count());
    }

    [Fact]
    public void Triggers_HolidayApprovalWorkflow_Returns3Triggers()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<string> triggers = workflow.Triggers;

      // Assert
      Assert.NotNull(triggers);
      Assert.Equal(3, triggers.Count());
    }

    [Fact]
    public void Transitions_HolidayApprovalWorkflow_Returns3Transitions()
    {
      // Arrange
      var workflow = new HolidayApprovalWorkflow();

      // Act
      List<Transition> transitions = workflow.Transitions;

      // Assert
      Assert.NotNull(transitions);
      Assert.Equal(3, transitions.Count());
    }
  }
}