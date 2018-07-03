
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public static class WorkflowServiceExtension
  {
    public static WorkflowTriggerInfo ToWorkflowTriggerInfo(
      this IWorkflowEngine workflowEngine,
      IWorkflow instance,
      TriggerResult result = null)
    {
      WorkflowTriggerInfo info;
      if (result == null || !result.HasErrors)
      {
        IEnumerable<TriggerResult> triggerResults = workflowEngine.GetTriggers(instance);
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