using System.Collections.Generic;
using System.Linq;

namespace tomware.MicroWF.Execution
{
  public class TriggerResult
  {
    private readonly TransitionContext _triggerContext;

    /// <summary>
    /// Indicates whether the transition can be triggered
    /// </summary>
    public bool CanTrigger { get; private set; }

    /// <summary>
    /// Indicates whether a trigger was aborted
    /// </summary>
    public bool IsAborted { get; private set; }

    /// <summary>
    /// Trigger that should be triggered
    /// </summary>
    public string TriggerName { get; private set; }

    private bool HasErrors
    {
      get { return Errors != null && Errors.Count() > 0; }
    }

    /// <summary>
    /// Trigger errors that occured during trying to make the transition
    /// </summary>
    public IEnumerable<string> Errors { get; private set; }

    /// <summary>
    /// Returns the current state of the IWorkflow instance
    /// </summary>
    public string CurrentState
    {
      get { return _triggerContext.Workflow.State; }
    }

    public TriggerResult(string triggerName, TransitionContext context, bool canTrigger)
    {
      _triggerContext = context;

      TriggerName = triggerName;
      Errors = _triggerContext.Errors;
      IsAborted = _triggerContext.TransitionAborted;
      CanTrigger = canTrigger;
    }

    public T GetVariable<T>(string key) where T : WorkflowVariableBase
    {
      if (_triggerContext == null) return null;

      return _triggerContext.GetVariable<T>(key);
    }
  }
}
