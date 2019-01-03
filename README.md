[![Build Status](https://travis-ci.org/thomasduft/microwf.svg?branch=master)](https://travis-ci.org/thomasduft/microwf)

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
Assuming you downloaded the sources and opened the directory with [VS Code](https://code.visualstudio.com/) you should be good to go! Ahh and of course you need [dot.net core](https://dotnet.microsoft.com/download) and [node.js](https://nodejs.org/en/) installed on your development environment.

#### Running the WebApi backend
1. Within the directory open the terminal and type
> dotnet build

That ensures you are able to build the dotnet related stuff!

2. Hit "F5" and VS Code will do its magic! There are some VS Code tasks defined which will run the WebApi and open your default browser.

#### Running the Angular WebClient frontend
1. In the terminal change to the directory
> cd samples/WebClient

2. Install all npm deps
> npm install

3. Run the application
> npm run start

You should see now the login screen. 

Happy poking!!
