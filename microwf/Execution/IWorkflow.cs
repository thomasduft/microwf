namespace microwf.Execution
{
  public interface IWorkflow
  {
    string Type { get; }
    string State { get; set; }
  }
}
