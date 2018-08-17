using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Core;

namespace microwf.Tests.Core
{
  [TestClass]
  public class TransitionTest
  {
    [TestMethod]
    public void Transition_NewInstance_CanMakeTransitionDefaultsToTrue()
    {
      // Arrange
      
      // Act
      var transition = new Transition();

      // Assert
      Assert.IsNotNull(transition);
      Assert.IsTrue(transition.CanMakeTransition(new TransitionContext(new Switcher())));
      Assert.IsNull(transition.BeforeTransition);
      Assert.IsNull(transition.AfterTransition);
    }
  }
}