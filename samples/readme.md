This sample application demonstrates the usage of the `microwf` libraries in a [.NET Web Api](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0). The WebApi sample application showcases some sample workflows that show possible scenarios like:
- holiday approval process (see `HolidayApprovalWorkflow`)
- issue or bug tracking process (see `IssueTrackingWorkflow`)
- non user interaction process (see `StepperWorkflow`)

You can login with the following three users:
- Admin (Administrator)
- Alice (Bob's boss when it comes to approve holidays)
- Bob

The password for all of them is: **password**

# Holiday approval and Issue tracking sample
Both sample workflows come with an angular based ui. Based on the users role you will have the possibilities to create (Alice), review, fix an issue (Admin) or create an holiday approval (Bob) that can be approved or rejected (Alice).

## Usage
1. Go to the VS Code Debug tab (Ctrl+Shift+D) and run the WebApi project and run it.

3. If wanted also run the ConsoleClient that triggers the Stepper sample. Just make sure the WebApi project runs first.

# Stepper sample
The stepper workflow makes use of the microwf's AutoTrigger and Worker (`JobQueueService`) feature.

Once triggered a Stepper instance with the trigger 'goto1' it automatically finishes itself without any further user interaction!

The sequence looks like the following where after step2 it pauses for one minute before continuing:
> trigger goto1 => step1 --> step2 --> step3 --> step4 --> step5

