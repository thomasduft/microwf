using System.Text;

namespace tomware.Microwf.Core
{
  public static class WorkflowDefinitionExtension
  {
    /// <summary>
    /// Exports the workflow definition to a dot directed graph.
    /// See: http://www.webgraphviz.com/ 
    /// </summary>
    /// <param name="workflow"></param>
    /// <param name="rankDir">Specifies the rank directory the graph will be plotted</param>
    /// <returns></returns>
    public static string ToDot(this IWorkflowDefinition workflow, string rankDir = "")
    {
      var sb = new StringBuilder();

      sb.AppendLine($"digraph {workflow.Type} {{");
      if (!string.IsNullOrEmpty(rankDir)) sb.AppendLine($"  rankdir = {rankDir};");
      foreach(var t in workflow.Transitions)
      {
        sb.AppendLine($"  {t.State} -> {t.TargetState} [ label = {t.Trigger} ];");
      }
      sb.AppendLine("}");

      return sb.ToString();
    }
  }
}
