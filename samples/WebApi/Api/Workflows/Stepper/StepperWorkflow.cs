using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using tomware.Microwf.Engine;

namespace WebApi.Workflows.Stepper
{
  /**
   * Showcase for JobQueueService.
   *
   * Once triggered a Stepper instance with 'goto1' the workflow automatically finishes itself
   * without any further user interaction!
   *
   * trigger goto1 => step1 --> step2 --> step3 --> step4 --> step5
   */
  public class StepperWorkflow : EntityWorkflowDefinitionBase
  {
    public const string TYPE = "StepperWorkflow";

    // Triggers
    public const string GOTO1_TRIGGER = "goto1";
    public const string GOTO2_TRIGGER = "goto2";
    public const string GOTO3_TRIGGER = "goto3";
    public const string GOTO4_TRIGGER = "goto4";
    public const string GOTO5_TRIGGER = "goto5";
    public const string CANCEL_TRIGGER = "cancel";

    // States
    public const string NEW_STATE = "new";
    public const string STEP1_STATE = "step1";
    public const string STEP2_STATE = "step2";
    public const string STEP3_STATE = "step3";
    public const string STEP4_STATE = "step4";
    public const string STEP5_STATE = "step5";
    public const string CANCELED_STATE = "canceled";

    public override string Type => TYPE;

    public override Type EntityType => typeof(Stepper);

    public override List<Transition> Transitions
    {
      get
      {
        return new List<Transition>
        {
          new Transition
          {
            State = NEW_STATE,
            Trigger = GOTO1_TRIGGER,
            TargetState = STEP1_STATE,
            CanMakeTransition = IsCreator,
            AfterTransition = GoToStep2,
            AutoTrigger = (ctx) => AutoTrigger(GOTO2_TRIGGER)
          },
          new Transition
          {
            State = STEP1_STATE,
            Trigger = GOTO2_TRIGGER,
            TargetState = STEP2_STATE,
            CanMakeTransition = IsAssignedToSystem,
            AfterTransition = AssignForStep3,
            AutoTrigger = (ctx) => AutoTrigger(GOTO3_TRIGGER, SystemTime.Now().AddMinutes(1))
          },
          new Transition
          {
            State = STEP1_STATE,
            Trigger = CANCEL_TRIGGER,
            TargetState = CANCELED_STATE
          },
          new Transition
          {
            State = STEP2_STATE,
            Trigger = GOTO3_TRIGGER,
            TargetState = STEP3_STATE,
            CanMakeTransition = IsAssignedToSystem,
            AfterTransition = GoToStep4,
            AutoTrigger = (ctx) => AutoTrigger(GOTO4_TRIGGER)
          },
          new Transition
          {
            State = STEP2_STATE,
            Trigger = CANCEL_TRIGGER,
            TargetState = CANCELED_STATE
          },
          new Transition
          {
            State = STEP3_STATE,
            Trigger = GOTO4_TRIGGER,
            TargetState = STEP4_STATE,
            CanMakeTransition = IsCreator,
            AfterTransition = GoTo5,
            AutoTrigger = (ctx) => AutoTrigger(GOTO5_TRIGGER)
          },
          new Transition
          {
            State = STEP3_STATE,
            Trigger = CANCEL_TRIGGER,
            TargetState = CANCELED_STATE
          },
          new Transition
          {
            State = STEP4_STATE,
            Trigger = GOTO5_TRIGGER,
            TargetState = STEP5_STATE,
            CanMakeTransition = IsAssignedToSystem,
            BeforeTransition = FailForFinish,
            AfterTransition = AssignToCreator
          },
          new Transition
          {
            State = STEP4_STATE,
            Trigger = CANCEL_TRIGGER,
            TargetState = CANCELED_STATE
          }
        };
      }
    }

    private void GoToStep2(TransitionContext context)
    {
      this.AssignToSystem(context);

      var stepper = context.GetInstance<Stepper>();
    }

    private void AssignForStep3(TransitionContext context)
    {
      this.AssignToSystem(context);
    }

    private void GoToStep4(TransitionContext context)
    {
      this.AssignToCreator(context);

      var stepper = context.GetInstance<Stepper>();
    }

    private void GoTo5(TransitionContext context)
    {
      this.AssignToSystem(context);

      var stepper = context.GetInstance<Stepper>();
    }

    private void FailForFinish(TransitionContext context)
    {
      // throw new NotImplementedException();
    }

    private bool IsCreator(TransitionContext context)
    {
      var stepper = context.GetInstance<Stepper>();

      return stepper.Assignee == stepper.Creator
        && stepper.Assignee != UserContextService.SYSTEM_USER;
    }

    private bool IsAssignedToSystem(TransitionContext context)
    {
      var stepper = context.GetInstance<Stepper>();

      return stepper.Assignee == UserContextService.SYSTEM_USER;
    }

    private void AssignToSystem(TransitionContext context)
    {
      var stepper = context.GetInstance<Stepper>();

      stepper.Assignee = UserContextService.SYSTEM_USER;
    }

    private void AssignToCreator(TransitionContext context)
    {
      var stepper = context.GetInstance<Stepper>();

      stepper.Assignee = stepper.Creator;
    }
  }
}