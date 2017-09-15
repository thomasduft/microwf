# microwf
A simple finite state machine (FSM) with workflow character where you define your workflows in code.

### Holiday approval sample
![Holiday Aproval](/holidayapproval.png)

In code it looks like:
```csharp
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
        new Trigger { Name = "Reject", DisplayName = "Reject" },
      };
    }
  }

  public List<Transition> Transitions
  {
    get
    {
      return new List<Transition>
      {
        new Transition {State = "New", Trigger = "Apply", TargetState = "Applied" },
        new Transition {State = "Applied", Trigger = "Approve", rgetState = "Approved"},
        new Transition {State = "Applied", Trigger = "Reject", rgetState = "Rejected"}
      };
    }
  }
}
```