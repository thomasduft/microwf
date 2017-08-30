using System;
using microwf.Execution;

namespace microwf.Definition
{
  public class Transition
  {
    public string State { get; set; }
    public string Trigger { get; set; }
    public string TargetState { get; set; }

    public Func<TriggerContext, bool> CanMakeTransition { get; set; }
    public Action<TriggerContext> BeforeTransition { get; set; }
    public Action<TriggerContext> AfterTransition { get; set; }

    public Transition()
    {
      CanMakeTransition = triggerContext => true;
    }
  }
}
