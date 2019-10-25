namespace tomware.Microwf.Engine
{
  public interface IAssignableWorkflow : IWorkflowInstanceEntity
  {
    /// <summary>
    /// The assigned subject for a workflow.
    /// </summary>
    string Assignee { get; set; }
  }
}
