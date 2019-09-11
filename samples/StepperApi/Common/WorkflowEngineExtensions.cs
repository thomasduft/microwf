using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace StepperApi.Common
{
  public static class WorkflowServiceExtension
  {
    public static async Task<WorkflowTriggerInfo> ToWorkflowTriggerInfo(
      this IWorkflowEngine workflowEngine,
      IWorkflow instance,
      TriggerResult result = null)
    {
      WorkflowTriggerInfo info;
      if (result == null || !result.HasErrors)
      {
        IEnumerable<TriggerResult> triggerResults = await workflowEngine.GetTriggersAsync(instance);
        var triggers = triggerResults.Select(x => x.TriggerName);
        info = WorkflowTriggerInfo.createForSuccess(triggers);
      }
      else
      {
        info = WorkflowTriggerInfo.createForError(result.Errors);
      }

      return info;
    }
  }
}