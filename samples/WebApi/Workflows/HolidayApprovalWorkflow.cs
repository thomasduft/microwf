using System.Collections.Generic;
using tomware.Microwf;
using WebApi.Domain;

namespace WebApi.Workflows
{
  public class HolidayApprovalWorkflow : WorkflowDefinitionBase
  {
    public const string NAME = "HolidayApprovalWorkflow";

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
            State = "New",
            Trigger = "Apply",
            TargetState ="Applied",
            CanMakeTransition = MeApplyingForHolidays
          },
          new Transition {
            State = "Applied",
            Trigger = "Approve",
            TargetState ="Approved",
            CanMakeTransition = BossIsApproving,
            AfterTransition = ThankBossForApproving
          },
          new Transition {
            State = "Applied",
            Trigger = "Reject",
            TargetState ="Rejected"
          }
        };
      }
    }

    public HolidayApprovalWorkflow()
    {
      // inject further dependencies if required i.e. CurrentUser 
    }

    private bool MeApplyingForHolidays(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      return holiday.Requestor == "Me";
    }

    private bool BossIsApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      return holiday.Superior == "NiceBoss";
    }

    private void ThankBossForApproving(TransitionContext context)
    {
      // SendMail("Thank you!!!");
    }
  }
}