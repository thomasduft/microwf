using System;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Tests.Common.WorkflowDefinitions
{
  public class OnOffWorkflow : WorkflowDefinitionBase
  {
    public const string TYPE = "OnOffWorkflow";

    public override string Type
    {
      get { return TYPE; }
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
            CanMakeTransition = CanSwitch,
            BeforeTransition = BeforeTransition,
            AfterTransition = AfterTransition
          },
          new Transition {
            State = "Off",
            Trigger = "SwitchOn",
            TargetState ="On",
            CanMakeTransition = CanSwitch,
            BeforeTransition = BeforeTransition,
            AfterTransition = AfterTransition,
            AutoTrigger = SwitchOffAfter5Minutes
          },
        };
      }
    }

    private bool CanSwitch(TransitionContext context)
    {
      var switcher = context.GetInstance<Switcher>();

      if (context.ContainsKey(SwitcherWorkflowVariable.KEY))
      {
        var variable = context
          .GetVariable<SwitcherWorkflowVariable>(SwitcherWorkflowVariable.KEY);

        return variable.CanSwitch;
      }

      return true;
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

    private AutoTrigger SwitchOffAfter5Minutes(TransitionContext context)
    {
      return new AutoTrigger { Trigger = "SwitchOff", DueDate = SystemTime.Now() };
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
      this.State = "Off";
      this.Type = OnOffWorkflow.TYPE;
    }
  }

  public class SwitcherWorkflowVariable : WorkflowVariableBase
  {
    public const string KEY = "SwitcherWorkflowVariable";

    public bool CanSwitch { get; set; }
    public SwitcherWorkflowVariable(bool canSwitch)
    {
      this.CanSwitch = canSwitch;
    }
  }
}