using tomware.Microwf.Engine;

namespace tomware.Microwf.Core
{
  public static class TriggerParamExtension
  {
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
    public static bool HasVariable<TVariable>(
      this TransitionContext transitionContext
    ) 
    {
      return transitionContext.ContainsKey(KeyBuilder.ToKey(typeof(TVariable)));
    }

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