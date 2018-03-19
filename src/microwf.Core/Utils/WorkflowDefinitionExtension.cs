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
    /// <returns></returns>
    public static string ToDot(this IWorkflowDefinition workflow, string rankDir = "LR")
    {
      var sb = new StringBuilder();

      sb.AppendLine($"digraph {workflow.Type} {{");
      sb.AppendLine($"  rankdir = {rankDir};");
      foreach(var t in workflow.Transitions)
      {
        sb.AppendLine($"  {t.State} -> {t.TargetState} [ label = {t.Trigger} ];");
      }
      sb.AppendLine("}");

      return sb.ToString();
    }
  }
}
