using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IAssignableWorkflow : IWorkflow
  {
    /// <summary>
    /// The assigned subject for a workflow.
    /// </summary>
    string Assignee { get; set; }
  }

  public interface IEntityWorkflow : IAssignableWorkflow
  {
    int Id { get; set; }
  }
}
