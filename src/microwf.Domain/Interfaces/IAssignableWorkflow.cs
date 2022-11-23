namespace tomware.Microwf.Domain
{
  public interface IAssignableWorkflow : IWorkflowInstanceEntity
  {
    /// <summary>
    /// The assigned subject for a workflow.
    /// </summary>
    string Assignee { get; set; }
  }
}