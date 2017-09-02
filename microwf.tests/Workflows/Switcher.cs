using microwf.Definition;

namespace microwf.tests.Workflows
{
  public class Switcher : IWorkflow
  {
    // IWorkflow properties
    public string State { get; set; }
    public string Type { get; set; }

    // some other properties
    public decimal Amount { get; set; }
    public int UserId { get; set; }

    public Switcher()
    {
      State = "Off";
    }
  }
}
