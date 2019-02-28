using System.Collections.Generic;
using System.Linq;

namespace tomware.Microwf.Core
{
  public abstract class WorkflowDefinitionBase : IWorkflowDefinition
  {
    private List<string> _states;
    private List<string> _triggers;

    /// <summary>
    /// The unique name for the workflow definition.
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    /// Returns a list of states for the workflow.
    /// </summary>
    public virtual List<string> States
    {
      get
      {
        if (_states != null)
        {
          return _states;
        }

        var states = Transitions
          .Select(t => t.State);

        var targetStates = Transitions
          .Select(t => t.TargetState);

        _states = states
          .Union(targetStates)
          .Distinct()
          .ToList();

        return _states;
      }
    }

    /// <summary>
    /// Returns a list of triggers for the workflow.
    /// </summary>
    public virtual List<string> Triggers
    {
      get
      {
        if (_triggers != null)
        {
          return _triggers;
        }

        _triggers = Transitions
          .Select(t => t.Trigger)
          .ToList();

        return _triggers;
      }
    }

    /// <summary>
    /// Returns a list of transitions for the workflow.
    /// </summary>
    public abstract List<Transition> Transitions { get; }
  }
}