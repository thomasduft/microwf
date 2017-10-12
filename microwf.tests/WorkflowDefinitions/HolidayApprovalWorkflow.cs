using microwf.Definition;
using microwf.Execution;
using System.Collections.Generic;

namespace microwf.tests.WorkflowDefinitions
{
  public class HolidayApprovalWorkflow : IWorkflowDefinition
  {
    public const string NAME = "HolidayApprovalWorkflow";

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
          new State { Name = "New", DisplayName = "New" },
          new State { Name = "Applied", DisplayName = "Applied" },
          new State { Name = "Approved", DisplayName = "Approved" },
          new State { Name = "Rejected", DisplayName = "Rejected" }
        };
      }
    }

    public List<Trigger> Triggers
    {
      get
      {
        return new List<Trigger>
        {
          new Trigger { Name = "Apply", DisplayName = "Apply" },
          new Trigger { Name = "Approve", DisplayName = "Approve" },
          new Trigger { Name = "Reject", DisplayName = "Reject" }
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

    private bool MeApplyingForHolidays(TriggerContext context)
    {
      var holiday = context.GetWorkflow<Holiday>();

      return holiday.Me == "Me";
    }

    private bool BossIsApproving(TriggerContext context)
    {
      var holiday = context.GetWorkflow<Holiday>();

      return holiday.Boss == "NiceBoss";
    }

    private void ThankBossForApproving(TriggerContext context)
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
      State = "New";
    }
  }
}
