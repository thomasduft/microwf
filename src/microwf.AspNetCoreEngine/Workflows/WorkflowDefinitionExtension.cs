using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tomware.Microwf.Engine;

namespace tomware.Microwf.Core
{
  public static class WorkflowDefinitionExtension
  {
    public static string ToDotWithHistory(
      this IWorkflowDefinition workflow,
      Workflow instance,
      string rankDir = ""
    )
    {
      var currentState = instance.State;
      var history = instance.WorkflowHistories;

      var sb = new StringBuilder();

      sb.AppendLine($"digraph {workflow.Type} {{");
      if (!string.IsNullOrEmpty(rankDir)) sb.AppendLine($"  rankdir = {rankDir};");

      // current
      sb.AppendLine($"  {currentState} [ style=\"filled\", color=\"#e95420\" ];");

      foreach (var t in workflow.Transitions)
      {
        var triggered = Exists(t, history);
        if (triggered)
        {
          sb.AppendLine($"  {t.State} -> {t.TargetState} [ label = {t.Trigger}, color =\"#e95420\", penwidth=2 ];");
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