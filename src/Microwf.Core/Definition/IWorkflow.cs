namespace tomware.Microwf.Core
{
  public interface IWorkflow
  {
    /// <summary>
    /// Defines a unique workflow type.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Defines the state of the workflow.
    /// </summary>
    string State { get; set; }
  }
}
