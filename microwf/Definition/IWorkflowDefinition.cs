using System.Collections.Generic;

namespace tomware.MicroWF.Definition
{
  public interface IWorkflowDefinition
  {
    /// <summary>
    /// Returns a unique the workflow type.
    /// </summary>
    string WorkflowType { get; }

    /// <summary>
    /// Returns a list of states for the workflow.
    /// </summary>
    List<string> States { get; }

    /// <summary>
    /// Returns a list of triggers for the workflow.
    /// </summary>
    List<string> Triggers { get; }

    /// <summary>
    /// Returns a list of transitions for the workflow.
    /// </summary>
    List<Transition> Transitions { get; }
  }
}
