using microwf.Definition;
using System.Collections.Generic;
using System.Linq;

namespace microwf.Execution
{
  /// <summary>
  /// Runtime for state transitions
  /// </summary>
  public class WorkflowExecution
  {
    private readonly IWorkflowDefinition _definition;

    public WorkflowExecution(IWorkflowDefinition definition)
    {
      _definition = definition;
    }

    /// <summary>
    /// Returns all possible triggers that can be made for the current state
    /// </summary>
    public IEnumerable<TriggerResult> GetTriggers(IWorkflow instance, Dictionary<string, WorkflowVariableBase> variables = null)
    {
      var context = CreateTriggerContext(instance, variables);

      return _definition.Transitions
        .Where(t => t.State == instance.State)
        .Select(t => CreateTriggerResult(t.Trigger, context, t)).ToList();
    }
    
    /// <summary>
    /// Checks whether a trigger can be executed
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public TriggerResult CanTrigger(TriggerParam param)
    {
      var context = CreateTriggerContext(param.Instance, param.Variables);

      return CanMakeTransition(context, param.TriggerName, param.Instance);
    }

    /// <summary>
    /// Triggers and executes a transition.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public TriggerResult Trigger(TriggerParam param)
    {
      var context = CreateTriggerContext(param.Instance, param.Variables);

      var result = CanMakeTransition(context, param.TriggerName, param.Instance);
      if (!result.CanTrigger) return result;

      var transition = GetTransition(param.TriggerName, param.Instance);
      if (context.TransitionAborted) return CreateTriggerResult(param.TriggerName, context, transition);

      transition.BeforeTransition?.Invoke(context);

      var state = _definition.States.Single(s => s == transition.TargetState);
      param.Instance.State = state;

      transition.AfterTransition?.Invoke(context);

      return result;
    }

    private static TriggerContext CreateTriggerContext(IWorkflow instance, Dictionary<string, WorkflowVariableBase> variables)
    {
      var context = new TriggerContext(instance);
      if (variables != null)
      {
        foreach (var variable in variables)
        {
          context.SetVariable(variable.Key, variable.Value);
        }
      }
      return context;
    }

    private static TriggerResult CreateTriggerResult(string triggerName, TriggerContext context, Transition transition)
    {
      return new TriggerResult(context, transition != null && transition.CanMakeTransition(context))
      {
        IsAborted = context.TransitionAborted,
        TriggerName = triggerName,
        TriggerErrors = context.Errors
      };
    }

    private Transition GetTransition(string triggerName, IWorkflow instance)
    {
      return _definition.Transitions
        .SingleOrDefault(t => t.Trigger == triggerName && t.State == instance.State);
    }

    private TriggerResult CanMakeTransition(TriggerContext context, string triggerName, IWorkflow instance)
    {
      var transition = GetTransition(triggerName, instance);
      var triggerResult = CreateTriggerResult(triggerName, context, transition);

      if (transition != null) return triggerResult;

      context.AddError($"Transition for trigger '{triggerName}' not found!");

      return triggerResult;
    }
  }
}
