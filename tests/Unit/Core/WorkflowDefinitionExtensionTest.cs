using System.Text;
using tomware.Microwf.Core;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using Xunit;

namespace tomware.Microwf.Tests.Unit.Core
{
  public class WorkflowDefinitionExtensionTest
  {
    [Fact]
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
      Assert.NotNull(diagraph);
      Assert.Equal(expected.ToString(), diagraph);
    }

    [Fact]
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
      Assert.NotNull(diagraph);
      Assert.Equal(expected.ToString(), diagraph);
    }
  }
}