using tomware.Microwf.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace microwf.tests.WorkflowDefinitions
{
  public class OnOffWorkflow : WorkflowDefinitionBase
  {
    public const string NAME = "OnOffWorkflow";

    public override string WorkflowType
    {
      get { return NAME; }
    }

    public override List<Transition> Transitions
    {
      get
      {
        return new List<Transition>
        {
          new Transition {
            State = "On",
            Trigger = "SwitchOff",
            TargetState ="Off",
            BeforeTransition = BeforeTransition,
            AfterTransition = AfterTransition
          },
          new Transition {
            State = "Off",
            Trigger = "SwitchOn",
            TargetState ="On",
            BeforeTransition = BeforeTransition,
            AfterTransition = AfterTransition
          },
        };
      }
    }

    private void BeforeTransition(TransitionContext context)
    {
      var switcher = context.GetInstance<Switcher>();

      Console.WriteLine("Current state is: '{0}'", switcher.State);
    }

    private void AfterTransition(TransitionContext context)
    {
      var switcher = context.GetInstance<Switcher>();
      
      Console.WriteLine("Current state is: '{0}'", switcher.State);
    }
  }

  public class Switcher : IWorkflow
  {
    // IWorkflow properties
    public string Type { get; set; }
    public string State { get; set; }

    // some other properties
    public int UserId { get; set; }

    public Switcher()
    {
      State = "Off";
    }
  }
}
