using System.Collections.Generic;
using System.Linq;

namespace microwf.Execution
{
  public class TriggerResult
  {
    private readonly TriggerContext _triggerContext;

    /// <summary>
    /// Indicates whether the trigger can be triggered
    /// </summary>
    public bool CanTrigger { get; private set; }

    /// <summary>
    /// Indicates whether a trigger was aborted
    /// </summary>
    public bool IsAborted { get; set; }

    /// <summary>
    /// Trigger that should be triggered
    /// </summary>
    public string TriggerName { get; set; }

    /// <summary>
    /// Trigger errors that occured during trying to make the transition
    /// </summary>
    public IEnumerable<string> TriggerErrors { get; set; }

    /// <summary>
    /// Returns the current state of the IWorkflow instance.
    /// </summary>
    public string CurrentState
    {
      get { return _triggerContext.Instance.State; }
    }

    private bool HasTriggerErrors
    {
      get { return TriggerErrors != null && TriggerErrors.Count() > 0; }
    }

    public TriggerResult(TriggerContext context, bool canTrigger)
    {
      _triggerContext = context;
      CanTrigger = canTrigger;
    }

    public T GetVariable<T>(string key) where T : WorkflowVariableBase
    {
      if (_triggerContext == null) return null;

      return _triggerContext.GetVariable<T>(key);
    }
  }
}
