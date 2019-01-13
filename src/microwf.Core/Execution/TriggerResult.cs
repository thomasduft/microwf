using System;
using System.Collections.Generic;
using System.Linq;

namespace tomware.Microwf.Core
{
  public class TriggerResult
  {
    private readonly TransitionContext _triggerContext;

    /// <summary>
    /// Indicates whether the transition can be triggered.
    /// </summary>
    public bool CanTrigger { get; private set; }

    /// <summary>
    /// Indicates whether a trigger was aborted.
    /// </summary>
    public bool IsAborted { get; private set; }

    /// <summary>
    /// Trigger that should be triggered.
    /// </summary>
    public string TriggerName { get; private set; }

    /// <summary>
    /// Indicates whether a trigger has errors.
    /// </summary>
    public bool HasErrors
    {
      get { return Errors != null && Errors.Count() > 0; }
    }

    /// <summary>
    /// Trigger errors that occured during trying to make the transition.
    /// </summary>
    public IEnumerable<string> Errors { get; }

    /// <summary>
    /// Returns the current state of the IWorkflow instance.
    /// </summary>
    public string CurrentState
    {
      get { return _triggerContext.Instance.State; }
    }

    public AutoTrigger AutoTrigger { get; private set; }

    public bool HasAutoTrigger
    {
      get
      {
        return this.AutoTrigger != null;
      }
    }

    public TriggerResult(string triggerName, TransitionContext context, bool canTrigger)
    {
      TriggerName = triggerName;
      _triggerContext = context;
      CanTrigger = canTrigger;
      IsAborted = _triggerContext.TransitionAborted;
      Errors = _triggerContext.Errors;
    }

    public T GetVariable<T>(string key) where T : WorkflowVariableBase
    {
      if (_triggerContext == null) return null;

      return _triggerContext.GetVariable<T>(key);
    }

    internal void SetAutoTrigger(AutoTrigger autoTrigger)
    {
      this.AutoTrigger = autoTrigger;
    }
  }
}
