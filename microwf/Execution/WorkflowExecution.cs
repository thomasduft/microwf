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
        .Select(t => CreateTriggerInfo(t.Trigger, context, t)).ToList();
    }

    public TriggerResult CanTrigger(TriggerParam param)
    {
      var context = CreateTriggerContext(param.Instance, param.Variables);

      return CanMakeTransition(context, param.TriggerName, param.Instance);
    }

    public TriggerResult Trigger(TriggerParam param)
    {
      var transition = GetTransition(param.TriggerName, param.Instance);
      var context = CreateTriggerContext(param.Instance, param.Variables);

      var triggerInfo = CanMakeTransition(context, param.TriggerName, param.Instance);
      if (!triggerInfo.CanTrigger) return triggerInfo;

      if (context.TransitionAborted) return CreateTriggerInfo(param.TriggerName, context, transition);

      transition.BeforeTransition?.Invoke(context);

      var state = _definition.States.Single(s => s.Name == transition.TargetState);
      param.Instance.State = state.Name;

      transition.AfterTransition?.Invoke(context);

      return triggerInfo;
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

    private static TriggerResult CreateTriggerInfo(string triggerName, TriggerContext context, Transition transition)
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
      var triggerInfo = CreateTriggerInfo(triggerName, context, transition);
      if (transition != null) return triggerInfo;

      context.AddError($"Transition for trigger '{triggerName}' not found!");

      return triggerInfo;
    }
  }
}
