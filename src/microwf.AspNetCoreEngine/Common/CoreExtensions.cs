using tomware.Microwf.Engine;

namespace tomware.Microwf.Core
{
  public static class TriggerParamExtension
  {
    /// <summary>
    /// Adds a workflow variable to the TriggerParam variables collection.
    /// </summary>
    /// <typeparam name="TVariable"></typeparam>
    /// <param name="triggerParam"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TriggerParam AddVariableWithKey<TVariable>(
      this TriggerParam triggerParam,
      WorkflowVariableBase value
    )
    {
      return triggerParam.AddVariable(KeyBuilder.ToKey(typeof(TVariable)), value);
    }
  }

  public static class TransitionContextExtension
  {
    /// <summary>
    /// Checks whether the workflow variable is present in the TransitionContext.
    /// </summary>
    /// <typeparam name="TVariable"></typeparam>
    /// <param name="transitionContext"></param>
    /// <returns></returns>
    public static bool HasVariable<TVariable>(
      this TransitionContext transitionContext
    )
    {
      return transitionContext.ContainsKey(KeyBuilder.ToKey(typeof(TVariable)));
    }

    /// <summary>
    /// Returns the workflow variable from the TransitionContext.
    /// </summary>
    /// <typeparam name="TVariable"></typeparam>
    /// <param name="transitionContext"></param>
    /// <returns></returns>
    public static TVariable ReturnVariable<TVariable>(
      this TransitionContext transitionContext
    ) where TVariable : WorkflowVariableBase
    {
      return transitionContext.GetVariable<TVariable>(
        KeyBuilder.ToKey(typeof(TVariable))
      );
    }
  }
}