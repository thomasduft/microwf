using System.Collections.Generic;
using System.Linq;
using System.Text;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  internal static class WorkflowDefinitionExtension
  {
    internal static string ToDotWithHistory(
      this IWorkflowDefinition workflow,
      Workflow instance,
      string rankDir = ""
    )
    {
      var history = instance.WorkflowHistories;

      var sb = new StringBuilder();

      sb.AppendLine($"digraph {workflow.Type} {{");
      if (!string.IsNullOrEmpty(rankDir)) sb.AppendLine($"  rankdir = {rankDir};");

      // sb.AppendLine($"  {instance.State} [ style=\"filled\", color=\"#e95420\" ];");

      foreach (var t in workflow.Transitions)
      {
        if (Exists(t, history))
        {
          sb.AppendLine($"  {t.State} -> {t.TargetState} " +
            $"[ label = {t.Trigger}, color =\"#e95420\", penwidth=3 ];");
        }
        else
        {
          sb.AppendLine($"  {t.State} -> {t.TargetState} [ label = {t.Trigger} ];");
        }
      }

      sb.AppendLine("}");

      return sb.ToString();
    }

    private static bool Exists(Transition t, List<WorkflowHistory> history)
    {
      return history.Any(h => h.FromState == t.State && h.ToState == t.TargetState);
    }
  }
}