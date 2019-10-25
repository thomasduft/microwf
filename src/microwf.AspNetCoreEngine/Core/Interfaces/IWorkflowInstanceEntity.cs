using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowInstanceEntity : IWorkflow
  {
    /// <summary>
    /// Id of an entity.
    /// </summary>
    int Id { get; set; }
  }
}
