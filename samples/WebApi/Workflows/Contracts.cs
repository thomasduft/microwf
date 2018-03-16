using tomware.Microwf;

namespace WebApi.Workflows
{
  public interface IWorkflowFactory
  {
    IWorkflow Create();
  }
}