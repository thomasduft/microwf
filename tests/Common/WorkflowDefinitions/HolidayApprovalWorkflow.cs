using System.Collections.Generic;
using tomware.Microwf.Core;

namespace tomware.Microwf.Tests.Common.WorkflowDefinitions
{
  public class HolidayApprovalWorkflow : WorkflowDefinitionBase
  {
    public const string TYPE = "HolidayApprovalWorkflow";

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
            State = "New",
            Trigger = "Apply",
            TargetState = "Applied",
            CanMakeTransition = MeApplyingForHolidays
          },
          new Transition {
            State = "Applied",
            Trigger = "Approve",
            TargetState = "Approved",
            CanMakeTransition = BossIsApproving,
            AfterTransition = ThankBossForApproving
          },
          new Transition {
            State = "Applied",
            Trigger = "Reject",
            TargetState = "Rejected"
          }
        };
      }
    }

    private bool MeApplyingForHolidays(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      return holiday.Me == "Me";
    }

    private bool BossIsApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      return holiday.Boss == "NiceBoss";
    }

    private void ThankBossForApproving(TransitionContext context)
    {
      // SendMail("Thank you!!!");
    }
  }

  public class Holiday : IWorkflow
  {
    // IWorkflow properties
    public string Type { get; set; }
    public string State { get; set; }

    // some other properties
    public string Me { get; set; }
    public string Boss { get; set; }

    public Holiday()
    {
      this.State = "New";
      this.Type = HolidayApprovalWorkflow.TYPE;
    }
  }
}