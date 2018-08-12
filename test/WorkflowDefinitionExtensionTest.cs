using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.tests.WorkflowDefinitions;
using tomware.Microwf.Core;
using System.Text;

namespace microwf.tests
{
  [TestClass]
  public class WorkflowDefinitionExtensionTest
  {
    [TestMethod]
    public void ToDot_IsOnOffWorkflow_ReturnsCorrectDotDiagraph()
    {
      // Arrange
      var onOffWorkflowDefinition = new OnOffWorkflow();

      var expected = new StringBuilder();
      expected.AppendLine("digraph OnOffWorkflow {");
      expected.AppendLine("  On -> Off [ label = SwitchOff ];");
      expected.AppendLine("  Off -> On [ label = SwitchOn ];");
      expected.AppendLine("}");

      // Act
      var diagraph = onOffWorkflowDefinition.ToDot();

      // Assert
      Assert.IsNotNull(diagraph);
      Assert.AreEqual(expected.ToString(), diagraph);
    }

    [TestMethod]
    public void ToDot_IsOnOffWorkflowWithLRRankDir_ReturnsCorrectDotDiagraph()
    {
      // Arrange
      var onOffWorkflowDefinition = new OnOffWorkflow();

      var expected = new StringBuilder();
      expected.AppendLine("digraph OnOffWorkflow {");
      expected.AppendLine("  rankdir = LR;");
      expected.AppendLine("  On -> Off [ label = SwitchOff ];");
      expected.AppendLine("  Off -> On [ label = SwitchOn ];");
      expected.AppendLine("}");

      // Act
      var diagraph = onOffWorkflowDefinition.ToDot("LR");

      // Assert
      Assert.IsNotNull(diagraph);
      Assert.AreEqual(expected.ToString(), diagraph);
    }
  }
}
