using microwf.Definition;
using microwf.Execution;
using microwf.tests.Workflows;
using System;
using System.Collections.Generic;

namespace microwf.tests.WorkflowDefinitions
{
  public class OnOffWorkflow : IWorkflowDefinition
  {
    public const string NAME = "OnOffWorkflow";

    public string WorkflowType
    {
      get { return NAME; }
    }

    public List<State> States
    {
      get
      {
        return new List<State>
        {
          new State { Name = "On", DisplayName = "On" },
          new State { Name = "Off", DisplayName = "Off" }
        };
      }
    }

    public List<Trigger> Triggers
    {
      get
      {
        return new List<Trigger>
        {
          new Trigger { Name = "SwitchOn", DisplayName = "Switch on" },
          new Trigger { Name = "SwitchOff", DisplayName = "Switch off" }
        };
      }
    }

    public List<Transition> Transitions
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

    private void BeforeTransition(TriggerContext context)
    {
      Console.WriteLine("Current state is: '{0}'", context.Instance.State);

      var switcher = context.Instance as Switcher;
      Console.WriteLine("Amount is: '{0}'", switcher.Amount);
    }

    private void AfterTransition(TriggerContext context)
    {
      Console.WriteLine("Current state is: '{0}'", context.Instance.State);
    }
  }
}
