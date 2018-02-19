using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using tomware.MicroWF.Tests.WorkflowDefinitions;
using tomware.MicroWF.Utils;

namespace tomware.MicroWF.Tests
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
      expected.AppendLine("  rankdir = LR;");
      expected.AppendLine("  On -> Off [ label = SwitchOff ];");
      expected.AppendLine("  Off -> On [ label = SwitchOn ];");
      expected.AppendLine("}");

      // Act
      var diagraph = onOffWorkflowDefinition.ToDot();

      // Assert
      Assert.IsNotNull(diagraph);
      Assert.AreEqual(expected.ToString(), diagraph);
    }
  }
}
