using System;

namespace tomware.Microwf.Core
{
  public class Transition
  {
    /// <summary>
    /// Current state before trigger happens.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Trigger that fires the transition.
    /// </summary>
    public string Trigger { get; set; }

    /// <summary>
    /// Target state after a successful transition.
    /// </summary>
    public string TargetState { get; set; }

    /// <summary>
    /// Decision point if a possible Transition can be made.
    /// </summary>
    public Func<TransitionContext, bool> CanMakeTransition { get; set; }

    /// <summary>
    /// Extension hook before the TargetState is beeing set.
    /// </summary>
    public Action<TransitionContext> BeforeTransition { get; set; }

    /// <summary>
    /// Extension hook after the TargetState has been set.
    /// </summary>
    public Action<TransitionContext> AfterTransition { get; set; }

    /// <summary>
    /// Extension hook to be set if a follow up Transition needs to be scheduled.
    /// </summary>
    public Func<TransitionContext, AutoTrigger> AutoTrigger { get; set; }

    public Transition()
    {
      CanMakeTransition = transitionContext => true;
    }
  }
}
