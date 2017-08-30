using microwf.Definition;
using System.Text;

namespace microwf.Utils
{
  public static class WorkflowDefinitionExtension
  {
    public static string ToDot(this IWorkflowDefinition workflow)
    {
      var sb = new StringBuilder();

      sb.AppendLine($"diagraph {workflow.WorkflowType} {{");
      foreach(var t in workflow.Transitions)
      {
        sb.AppendLine($"{t.State} -> {t.TargetState} [label={t.Trigger}]");
      }
      sb.AppendLine("}");

      return sb.ToString();
    }
  }
}
