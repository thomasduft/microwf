using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace WebApi.Common
{
  public static class WorkflowServiceExtension
  {
    public static async Task<WorkflowTriggerInfo> ToWorkflowTriggerInfo(
      this IWorkflowEngineService workflowEngine,
      IWorkflow instance,
      TriggerResult result = null)
    {
      WorkflowTriggerInfo info;
      if (result == null || !result.HasErrors)
      {
        IEnumerable<TriggerResult> triggerResults = await workflowEngine.GetTriggersAsync(instance);
        var triggers = triggerResults.Select(x => x.TriggerName);
        info = WorkflowTriggerInfo.Success(triggers);
      }
      else
      {
        info = WorkflowTriggerInfo.Error(result.Errors);
      }

      return info;
    }
  }
}