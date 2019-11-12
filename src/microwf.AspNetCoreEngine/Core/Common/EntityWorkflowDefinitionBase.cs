using System;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public abstract class EntityWorkflowDefinitionBase : WorkflowDefinitionBase
  {
    public abstract Type EntityType { get; }

    protected AutoTrigger AutoTrigger(
      TransitionContext contex,
      string trigger,
      DateTime? dueDate = null
    )
    {
      return new AutoTrigger
      {
        Trigger = trigger,
        DueDate = dueDate
      };
    }
  }
}