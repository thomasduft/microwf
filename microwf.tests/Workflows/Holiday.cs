using microwf.Execution;

namespace microwf.tests.Workflows
{
  public class Holiday : IWorkflow
  {
    // IWorkflow properties
    public string State { get; set; }
    public string Type { get; set; }

    // some other properties
    public decimal Amount { get; set; }
    public int UserId { get; set; }

    public Holiday()
    {
      State = "New";
    }
  }
}