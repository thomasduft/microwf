This sample application demonstrates the usage of the `microwf` libraries in a [.NET Web Api](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.0). It comes with some sample workflows that show possible scenarios like:
- holiday approval process (see `HolidayApprovalWorkflow`)
- issue or bug tracking process (see `IssueTrackingWorkflow`)
- non user interaction process (see `StepperWorkflow`)

# Holiday approval and Issue tracking sample
Both samples come with an angular based ui. Based on the users role you will have the possibilities to create (Alice), review, fix an issue (Admin) or create an holiday approval (Bob) that can be approved or rejected (Alice).

## Usage
1. In VS Code Debugger tab hit the ".NET Core Launch (WebApi)" button. 

2. In the terminal change to the directory
> cd samples/WebClient

3. Install all npm deps
> npm install

4. Run the application
> npm run start

5. Head over to http://localhost:4200. You should see now the login screen.


# Stepper sample
The stepper workflow makes use of the microwf's AutoTrigger and Worker (`JobQueueService`) feature.

Once triggered a Stepper instance with the trigger 'goto1' it automatically finishes itself without any further user interaction!

The sequence looks like the following where after step2 it pauses for one minutes before continuing:
> trigger goto1 => step1 --> step2 --> step3 --> step4 --> step5

## Usage
In VS Code Debugger tab hit the ".NET Core Launch (WebApi)" button. 

Once up and running hit the ".NET Core Launch (ConsoleClient)" button and watch the two console outputs or have a look in the logs.

