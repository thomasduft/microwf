using System.Collections.Generic;
using tomware.Microwf;
using WebApi.Domain;

namespace WebApi.Workflows
{
  public class HolidayApprovalWorkflow : WorkflowDefinitionBase
  {
    public const string NAME = "HolidayApprovalWorkflow";

    public const string APPLY_TRIGGER = "Apply";
    public const string APPROVE_TRIGGER = "Approve";
    public const string REJECT_TRIGGER = "Reject";

    public const string NEW_STATE = "New";
    public const string APPLIED_STATE = "Applied";
    public const string APPROVED_STATE = "Approved";
    public const string REJECTED_STATE = "Rejected";

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
            State = NEW_STATE,
            Trigger = APPLY_TRIGGER,
            TargetState = APPLIED_STATE,
            CanMakeTransition = MeApplyingForHolidays
          },
          new Transition {
            State = APPLIED_STATE,
            Trigger = APPROVE_TRIGGER,
            TargetState = APPROVED_STATE,
            CanMakeTransition = BossIsApproving,
            AfterTransition = ThankBossForApproving
          },
          new Transition {
            State = APPLIED_STATE,
            Trigger = REJECT_TRIGGER,
            TargetState = REJECTED_STATE
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