using System;
using tomware.MicroWF.Execution;

namespace tomware.MicroWF.Definition
{
  public class Transition
  {
    public string State { get; set; }
    public string Trigger { get; set; }
    public string TargetState { get; set; }

    public Func<TransitionContext, bool> CanMakeTransition { get; set; }
    public Action<TransitionContext> BeforeTransition { get; set; }
    public Action<TransitionContext> AfterTransition { get; set; }

    public Transition()
    {
      CanMakeTransition = triggerContext => true;
    }
  }
}
