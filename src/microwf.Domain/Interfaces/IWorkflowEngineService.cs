using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public interface IWorkflowEngineService
  {
    /// <summary>
    /// Returns the possible triggers for certain workflow instance.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    );

    /// <summary>
    /// Checks whether a certain trigger can be executed.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<TriggerResult> CanTriggerAsync(TriggerParam param);

    /// <summary>
    /// Triggers the desired trigger for a certain workflow instance.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<TriggerResult> TriggerAsync(TriggerParam param);

    /// <summary>
    /// Returns the desired workflow instance if existing.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    IWorkflow Find(int id, Type type);
  }
}