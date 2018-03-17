using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using tomware.Microwf;
using WebApi.Domain;

namespace WebApi.Workflows
{
  public class HolidayApprovalWorkflow : WorkflowDefinitionBase
  {
    private readonly ILogger<HolidayApprovalWorkflow> _logger;

    public const string NAME = "HolidayApprovalWorkflow";

    public const string APPLY_TRIGGER = "apply";
    public const string APPROVE_TRIGGER = "approve";
    public const string REJECT_TRIGGER = "reject";

    public const string NEW_STATE = "new";
    public const string APPLIED_STATE = "applied";
    public const string APPROVED_STATE = "approved";
    public const string REJECTED_STATE = "rejected";

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

    public HolidayApprovalWorkflow(ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<HolidayApprovalWorkflow>();

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

      _logger.LogInformation($"Holiday entity: {holiday.Superior}");

      return holiday.Superior == "NiceBoss";
    }

    private void ThankBossForApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      _logger.LogInformation($"Thank you very much: {holiday.Superior}!");
    }
  }
}