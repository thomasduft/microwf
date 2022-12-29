[![build](https://github.com/thomasduft/microwf/workflows/build/badge.svg)](https://github.com/thomasduft/microwf/actions) [![NuGet Release](https://img.shields.io/nuget/vpre/tomware.Microwf.AspNetCoreEngine.svg)](https://www.nuget.org/packages/tomware.Microwf.AspNetCoreEngine) [![BuitlWithDot.Net shield](https://builtwithdot.net/project/351/microwf-a-simple-finite-state-machine-fsm-with-workflow-character-where-you-define-your-workflows-in-code./badge)](https://builtwithdot.net/project/351/microwf-a-simple-finite-state-machine-fsm-with-workflow-character-where-you-define-your-workflows-in-code.)

# microwf

A simple finite state machine (FSM) with workflow character where you define your workflows in code.

### Holiday approval sample

![Holiday Aproval](/holidayapproval.png)

In code it looks like:

```csharp
public class HolidayApprovalWorkflow : WorkflowDefinitionBase
{
  public override string Type => nameof(HolidayApprovalWorkflow);

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

2. Hit F5 or go to the VS Code Debug tab (Ctrl+Shift+D) and run the WebApi project.

3. Once the project has started head over to your browser of choice and type in https://localhost:5001.

You should see now the login screen.

> For developing purpose there exists a compound task named `dev:be-fe 🚀` that spins up the WebApi project with the `dotnet watch run` command as well as the [Angular](https://angular.io/) based WebClient project with the `npm run start` command. 

### Administrator Web UI

A web interface allows an administrator to search for workflow instances and have a look into the current state.

![Admin](https://www.tomware.ch/2018/11/27/sample-workflow-system-with-asp-net-core-angular-and-microwf-explained-new-ui/admin.gif)

Happy poking!!
