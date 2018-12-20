using tomware.Microwf.Core;

namespace WebApi.Workflows.Stepper
{
  public class StepperViewModel : WorkflowVariableBase
  {
    public int Id { get; set; }

    public string Name { get; set; }
  }

  public class ProcessStepViewModel
  {
    public int Id { get; set; }

    public string Trigger { get; set; }
  }
}