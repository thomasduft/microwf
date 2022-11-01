using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Issue
{
  public class IssueTrackingWorkflow : EntityWorkflowDefinitionBase
  {
    private readonly ILogger<IssueTrackingWorkflow> _logger;

    public const string TYPE = "IssueTrackingWorkflow";

    // Triggers
    public const string ASSIGN_TRIGGER = "assign";
    public const string FIX_TRIGGER = "fix";
    public const string REVIEW_TRIGGER = "review";
    public const string APPROVE_TRIGGER = "approve";
    public const string CLOSE_TRIGGER = "close";
    public const string REJECT_TRIGGER = "reject";

    // States
    public const string OPEN_STATE = "open";
    public const string ASSIGNED_STATE = "assigned";
    public const string IN_PROGRESS_STATE = "in_progress";
    public const string RESOLVED_STATE = "resolved";
    public const string CLOSED_STATE = "closed";

    public override string Type => TYPE;

    public override Type EntityType => typeof(Issue);

    public override List<Transition> Transitions
    {
      get
      {
        return new List<Transition>
        {
          new Transition {
            State = OPEN_STATE,
            Trigger = ASSIGN_TRIGGER,
            TargetState = ASSIGNED_STATE,
            AfterTransition = AssignToAdmin
          },
          new Transition {
            State = ASSIGNED_STATE,
            Trigger = FIX_TRIGGER,
            TargetState = IN_PROGRESS_STATE
          },
          new Transition {
            State = IN_PROGRESS_STATE,
            Trigger = REVIEW_TRIGGER,
            TargetState = RESOLVED_STATE,
            AfterTransition = AssignToCreator
          },
          new Transition {
            State = RESOLVED_STATE,
            Trigger = APPROVE_TRIGGER,
            TargetState = CLOSED_STATE
          },
          new Transition {
            State = OPEN_STATE,
            Trigger = REVIEW_TRIGGER,
            TargetState = RESOLVED_STATE,
            AfterTransition = AssignToCreator
          },
          new Transition {
            State = ASSIGNED_STATE,
            Trigger = REVIEW_TRIGGER,
            TargetState = RESOLVED_STATE,
            AfterTransition = AssignToCreator
          },
          new Transition {
            State = OPEN_STATE,
            Trigger = CLOSE_TRIGGER,
            TargetState = CLOSED_STATE
          },
          new Transition {
            State = RESOLVED_STATE,
            Trigger = REJECT_TRIGGER,
            TargetState = IN_PROGRESS_STATE,
            AfterTransition = AssignToAdmin
          }
        };
      }
    }

    public IssueTrackingWorkflow(ILoggerFactory loggerFactory)
    {
      this._logger = loggerFactory.CreateLogger<IssueTrackingWorkflow>();
    }

    private void AssignToAdmin(TransitionContext context)
    {
      // because admin is the dev ;-)...
      var issue = context.GetInstance<Issue>();

      issue.Assignee = "Admin";

      this._logger.LogInformation($"Assignee: {issue.Assignee}");
    }

    private void AssignToCreator(TransitionContext context)
    {
      var issue = context.GetInstance<Issue>();

      issue.Assignee = issue.Creator;

      this._logger.LogInformation($"Assignee: {issue.Assignee}");
    }
  }
}