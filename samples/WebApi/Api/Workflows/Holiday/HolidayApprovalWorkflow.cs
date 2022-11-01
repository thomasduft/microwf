using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Holiday
{
  public class HolidayApprovalWorkflow : EntityWorkflowDefinitionBase
  {
    private readonly ILogger<HolidayApprovalWorkflow> _logger;
    private readonly IUserContextService _userContextService;

    public const string TYPE = "HolidayApprovalWorkflow";

    // Triggers
    public const string APPLY_TRIGGER = "apply";
    public const string APPROVE_TRIGGER = "approve";
    public const string REJECT_TRIGGER = "reject";

    // States
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
            BeforeTransition = AddAprovalMessage,
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
      IUserContextService userContextService
    )
    {
      this._logger = loggerFactory.CreateLogger<HolidayApprovalWorkflow>();
      this._userContextService = userContextService;
    }

    private void AssignBoss(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      if (context.HasVariable<ApplyHolidayViewModel>())
      {
        var model = context.ReturnVariable<ApplyHolidayViewModel>();
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

      return holiday.Superior == this._userContextService.UserName;
    }

    private void AddAprovalMessage(TransitionContext context)
    {
      var holiday = context.GetInstance<Holiday>();

      if (context.HasVariable<ApproveHolidayViewModel>())
      {
        var model = context.ReturnVariable<ApproveHolidayViewModel>();
        if (!string.IsNullOrWhiteSpace(model.Message))
        {
          holiday.AddMessage(this._userContextService.UserName, model.Message);
        }
      }
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

      this._logger.LogInformation($"Reassign Holiday entity to requestor: {holiday.Requester}");

      holiday.Assignee = holiday.Requester;

      if (context.HasVariable<ApproveHolidayViewModel>())
      {
        var model = context.ReturnVariable<ApproveHolidayViewModel>();
        if (!string.IsNullOrWhiteSpace(model.Message))
        {
          holiday.AddMessage(this._userContextService.UserName, model.Message);
        }
      }
    }
  }
}