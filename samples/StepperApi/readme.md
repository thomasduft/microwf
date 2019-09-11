# Stepper Sample
This sample demonstrate some own doggfooding. It directly depends on the nuget packages instead of referencing the project.

The stepper workflow makes use of the microwf's AutoTrigger and Worker (`JobQueueService`) feature.

Once triggered a Stepper instance with the trigger 'goto1' it automatically finishes itself without any further user interaction!

The sequence looks like the following where after step2 it pauses for two minutes before continuing:
> trigger goto1 => step1 --> step2 --> step3 --> step4 --> step5

## Usage
In VS Code Debugger tab hit the ".NET Core Launch (Stepper API)" button. 

Once up and running hit the ".NET Core Launch (ConsoleClient)" button and watch the two console outputs or have a look in the logs.