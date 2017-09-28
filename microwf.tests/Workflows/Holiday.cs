using microwf.Definition;

namespace microwf.tests.Workflows
{
  public class Holiday : IWorkflow
  {
    // IWorkflow properties
    public string State { get; set; }
    public string Type { get; set; }

    // some other properties
    public string Me { get; set; }
    public string Boss { get; set; }

    public Holiday()
    {
      State = "New";
    }
  }
}