using tomware.Microwf.Core;

namespace WebApi.Engine.Workflows
{
  public interface IEntityWorkflow : IAssignableWorkflow
  {
    int Id { get; set; }
  }
}
