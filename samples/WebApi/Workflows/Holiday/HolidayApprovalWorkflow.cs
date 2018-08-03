using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Common;

namespace WebApi.Workflows.Holiday
{
  public class HolidayApprovalWorkflow : EntityWorkflowDefinitionBase
  {
    private readonly ILogger<HolidayApprovalWorkflow> _logger;
    private readonly UserContextService _userContextService;

    public const string TYPE = "HolidayApprovalWorkflow";

    public const string APPLY_TRIGGER = "apply";
    public const string APPROVE_TRIGGER = "approve";
    public const string REJECT_TRIGGER = "reject";

    public const string NEW_STATE = "new";
    public const string APPLIED_STATE = "applied";
    public const string APPROVED_STATE = "approved";
    public const string REJECTED_STATE = "rejected";

    public override string Type => TYPE;

    public override Type EntityType => typeof(Holiday);

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
            TargetState = REJECTED_STATE,
            AfterTransition = ReAssignToRequestor
          }
        };
      }
    }

    public HolidayApprovalWorkflow(
      ILoggerFactory loggerFactory,
      UserContextService userContextService
    )
    {
      this._logger = loggerFactory.CreateLogger<HolidayApprovalWorkflow>();
      this._userContextService = userContextService;
    }

    private void AssignBoss(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      var key = KeyBuilder.ToKey(typeof(ApplyHolidayViewModel));
      if (context.ContainsKey(key))
      {
        var model = context.GetVariable<ApplyHolidayViewModel>(key);
        holiday.Assignee = holiday.Superior;
        holiday.From = model.From;
        holiday.To = model.To;

        if (!string.IsNullOrWhiteSpace(model.Message))
        {
          holiday.AddMessage(this._userContextService.UserName, model.Message);
        }
      }

      this._logger.LogInformation($"Assignee: {holiday.Assignee}");
    }

    private bool BossIsApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      this._logger.LogInformation($"Holiday entity in BossIsApproving: {holiday.Superior}");

      var key = KeyBuilder.ToKey(typeof(ApproveHolidayViewModel));
      if (context.ContainsKey(key))
      {
        var model = context.GetVariable<ApproveHolidayViewModel>(key);
        if (!string.IsNullOrWhiteSpace(model.Message))
        {
          holiday.AddMessage(this._userContextService.UserName, model.Message);
        }

        return holiday.Superior == this._userContextService.UserName;
      }

      return false;
    }

    private void ThankBossForApproving(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      this._logger.LogInformation($"Thank you very much: {holiday.Superior}!");

      holiday.Assignee = holiday.Requester;
    }

    private void ReAssignToRequestor(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      this._logger.LogInformation($"Reassign Holiday entity to requostor: {holiday.Requester}");

      holiday.Assignee = holiday.Requester;

      var key = KeyBuilder.ToKey(typeof(ApproveHolidayViewModel));
      if (context.ContainsKey(key))
      {
        var model = context.GetVariable<ApproveHolidayViewModel>(key);
        if (!string.IsNullOrWhiteSpace(model.Message))
        {
          holiday.AddMessage(this._userContextService.UserName, model.Message);
        }
      }
    }
  }
}