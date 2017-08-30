using System.Collections.Generic;

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
    /// Indicates whether a trigger was aborted.
    /// </summary>
    public bool IsAborted { get; set; }

    /// <summary>
    /// Trigger that should be triggered
    /// </summary>
    public string TriggerName { get; set; }

    /// <summary>
    /// Errors that will happen if trigger cannot be triggered.
    /// </summary>
    public IEnumerable<string> TriggerErrors { get; set; }

    private bool HasTriggerContext
    {
      get { return _triggerContext != null; }
    }

    public TriggerResult(TriggerContext context, bool canTrigger)
    {
      _triggerContext = context;
      CanTrigger = canTrigger;
    }

    /// <summary>
    /// Access to the workflow variables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetTriggerContextValue<T>(string key) where T : WorkflowVariableBase
    {
      if (!HasTriggerContext) return null;

      return _triggerContext.GetVariable<T>(key);
    }
  }
}
