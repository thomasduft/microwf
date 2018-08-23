using tomware.Microwf.Core;
using System;
using System.Collections.Generic;
using tomware.Microwf.Engine;
using System.ComponentModel.DataAnnotations;

namespace microwf.Tests.WorkflowDefinitions
{
  public class EntityOnOffWorkflow : EntityWorkflowDefinitionBase
  {
    public const string TYPE = "EntityOnOffWorkflow";

    public override string Type => TYPE;

    public override Type EntityType => typeof(LigthtSwitcher);

    public override List<Transition> Transitions
    {
      get
      {
        return new List<Transition>
        {
          new Transition {
            State = "On",
            Trigger = "SwitchOff",
            TargetState ="Off"
          },
          new Transition {
            State = "Off",
            Trigger = "SwitchOn",
            TargetState ="On"
          },
        };
      }
    }
  }

  public class LigthtSwitcher : IEntityWorkflow
  {
    [Key]
    public int Id { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public string Assignee { get; set; }

    public LigthtSwitcher()
    {
      State = "Off";
      Type = EntityOnOffWorkflow.TYPE;
    }
  }
}
