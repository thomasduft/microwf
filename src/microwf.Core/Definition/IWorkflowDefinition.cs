using System.Collections.Generic;

namespace tomware.Microwf.Core
{
  public interface IWorkflowDefinition
  {
    /// <summary>
    /// The unique name for the workflow definition.
    /// </summary>
    string Type { get; }

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
