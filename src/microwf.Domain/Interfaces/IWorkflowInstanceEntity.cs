using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public interface IWorkflowInstanceEntity : IWorkflow
  {
    /// <summary>
    /// Id of an entity.
    /// </summary>
    int Id { get; set; }
  }
}