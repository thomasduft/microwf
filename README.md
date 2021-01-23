[![build](https://github.com/thomasduft/microwf/workflows/build/badge.svg)](https://github.com/thomasduft/microwf/actions) [![NuGet Release](https://img.shields.io/nuget/vpre/tomware.Microwf.AspNetCoreEngine.svg)](https://www.nuget.org/packages/tomware.Microwf.AspNetCoreEngine) [![BuitlWithDot.Net shield](https://builtwithdot.net/project/351/microwf-a-simple-finite-state-machine-fsm-with-workflow-character-where-you-define-your-workflows-in-code./badge)](https://builtwithdot.net/project/351/microwf-a-simple-finite-state-machine-fsm-with-workflow-character-where-you-define-your-workflows-in-code.)

# microwf
A simple finite state machine (FSM) with workflow character where you define your workflows in code.

### Holiday approval sample
![Holiday Aproval](/holidayapproval.png)

In code it looks like:
```csharp
public class HolidayApprovalWorkflow : WorkflowDefinitionBase
{
  public const string TYPE = "HolidayApprovalWorkflow";

  public override string Type
  {
    get { return TYPE; }
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

### Running the samples
Assuming you downloaded the sources and opened the directory with [VS Code](https://code.visualstudio.com/) you should be good to go! Ahh and of course you need [.NET Core](https://dotnet.microsoft.com/download) and [node.js](https://nodejs.org/en/) installed on your development environment.

#### Running the WebApi backend
1. Open the integrated terminal in VS Code and type
> dotnet build

That ensures you are able to build the dotnet related stuff!

2. Go to the VS Code Debug tab (Ctrl+Shift+D) and run the Security Token Server (STS = [IdentityServer](https://identityserver.io/)) project.

3. After the STS is running change the dropdown to the WebApi project and run it.

You should see now the login screen.

Happy poking!!
