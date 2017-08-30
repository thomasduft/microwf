using System.Collections.Generic;

namespace microwf.Definition
{
  public interface IWorkflowDefinition
  {
    string WorkflowType { get; }
    List<State> States { get; }
    List<Transition> Transitions { get; }
    List<Trigger> Triggers { get; }
  }
}
