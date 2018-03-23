using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IEntityWorkflow : IAssignableWorkflow
  {
    int Id { get; set; }
  }
}
