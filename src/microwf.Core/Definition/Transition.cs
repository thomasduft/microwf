using System;

namespace tomware.Microwf.Core
{
  public class Transition
  {
    public string State { get; set; }
    public string Trigger { get; set; }
    public string TargetState { get; set; }

    public Func<TransitionContext, bool> CanMakeTransition { get; set; }
    public Action<TransitionContext> BeforeTransition { get; set; }
    public Action<TransitionContext> AfterTransition { get; set; }
    public Func<TransitionContext, AutoTrigger> AutoTrigger { get; set; }

    public Transition()
    {
      CanMakeTransition = transitionContext => true;
    }
  }
}
