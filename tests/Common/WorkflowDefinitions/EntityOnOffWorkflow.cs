using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Tests.Common.WorkflowDefinitions
{
  public class EntityOnOffWorkflow : EntityWorkflowDefinitionBase
  {
    public const string TYPE = "EntityOnOffWorkflow";

    public override string Type => TYPE;

    public override Type EntityType => typeof(LightSwitcher);

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
            AfterTransition = AfterTransition
          },
          new Transition {
            State = "Off",
            Trigger = "SwitchOn",
            TargetState ="On",
            CanMakeTransition = CanSwitch,
            AfterTransition = AfterTransition
          }
        };
      }
    }

    private bool CanSwitch(TransitionContext context)
    {
      if (context.HasVariable<LightSwitcherWorkflowVariable>())
      {
        var variable = context.ReturnVariable<LightSwitcherWorkflowVariable>();

        return variable.CanSwitch;
      }

      return true;
    }

    private void AfterTransition(TransitionContext context)
    {
      if (context.HasVariable<LightSwitcherWorkflowVariable>())
      {
        var variable = context.ReturnVariable<LightSwitcherWorkflowVariable>();

        variable.CanSwitch = !variable.CanSwitch;
      }
    }
  }

  public class LightSwitcher : IAssignableWorkflow
  {
    [Key]
    public int Id { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public string Assignee { get; set; }

    public LightSwitcher()
    {
      this.State = "Off";
      this.Type = EntityOnOffWorkflow.TYPE;
    }
  }

  public class LightSwitcherWorkflowVariable : WorkflowVariableBase
  {
    public bool CanSwitch { get; set; }
  }
}