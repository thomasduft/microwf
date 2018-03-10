[![Build Status](https://travis-ci.org/thomasduft/microwf.svg?branch=master)](https://travis-ci.org/thomasduft/microwf)

# microwf
A simple finite state machine (FSM) with workflow character where you define your workflows in code.

### Holiday approval sample
![Holiday Aproval](/holidayapproval.png)

In code it looks like:
```csharp
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
```