using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace WebApi.Workflows.Holiday
{
  public class HolidayApprovalWorkflow : EntityWorkflowDefinitionBase
  {
    private readonly ILogger<HolidayApprovalWorkflow> _logger;

    public const string TYPE = "HolidayApprovalWorkflow";

    public const string APPLY_TRIGGER = "apply";
    public const string APPROVE_TRIGGER = "approve";
    public const string REJECT_TRIGGER = "reject";

    public const string NEW_STATE = "new";
    public const string APPLIED_STATE = "applied";
    public const string APPROVED_STATE = "approved";
    public const string REJECTED_STATE = "rejected";

    public override string Type => TYPE;

    public override Type EntityType => typeof(Domain.Holiday);

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
            CanMakeTransition = MeApplyingForHolidays,
            AfterTransition = AssignBoss
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
      this._logger = loggerFactory.CreateLogger<HolidayApprovalWorkflow>();

      // inject further dependencies if required i.e. CurrentUser 
    }

    private bool MeApplyingForHolidays(TransitionContext context)
    {
      var holiday = context.GetInstance<Domain.Holiday>();
      var canApply = holiday.Requestor == "Me";

      this._logger.LogInformation($"Can apply: {canApply}");

      return canApply;
    }

    private void AssignBoss(TransitionContext context)
    {
      var holiday = context.GetInstance<Domain.Holiday>();
      holiday.Assignee = holiday.Superior;

      this._logger.LogInformation($"Assignee: {holiday.Assignee}");
    }

    private bool BossIsApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Domain.Holiday>();

      this._logger.LogInformation($"Holiday entity in BossIsApproving: {holiday.Superior}");

      return holiday.Superior == "NiceBoss";
    }

    private void ThankBossForApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Domain.Holiday>();

      this._logger.LogInformation($"Thank you very much: {holiday.Superior}!");
    }
  }
}