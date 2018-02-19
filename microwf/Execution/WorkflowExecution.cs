using System.Collections.Generic;
using System.Linq;
using tomware.MicroWF.Definition;

namespace tomware.MicroWF.Execution
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
    public IEnumerable<TriggerResult> GetTriggers(
      IWorkflow workflow,
      Dictionary<string, WorkflowVariableBase> variables = null)
    {
      var context = CreateTransitionContext(workflow, variables);

      return _definition.Transitions
        .Where(t => t.State == workflow.State)
        .Select(t => CreateTriggerResult(t.Trigger, context, t)).ToList();
    }

    /// <summary>
    /// Checks whether a trigger can be executed
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public TriggerResult CanTrigger(TriggerParam param)
    {
      var context = CreateTransitionContext(param.Workflow, param.Variables);

      return CanMakeTransition(context, param.TriggerName, param.Workflow);
    }

    /// <summary>
    /// Triggers and executes a transition.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public TriggerResult Trigger(TriggerParam param)
    {
      var context = CreateTransitionContext(param.Workflow, param.Variables);

      var result = CanMakeTransition(context, param.TriggerName, param.Workflow);
      if (!result.CanTrigger) return result;

      var transition = GetTransition(param.TriggerName, param.Workflow);
      if (context.TransitionAborted)
        return CreateTriggerResult(param.TriggerName, context, transition);

      transition.BeforeTransition?.Invoke(context);

      var state = _definition.States.Single(s => s == transition.TargetState);
      param.Workflow.State = state;

      transition.AfterTransition?.Invoke(context);

      return result;
    }

    private static TransitionContext CreateTransitionContext(
      IWorkflow workflow,
      Dictionary<string, WorkflowVariableBase> variables)
    {
      var context = new TransitionContext(workflow);
      if (variables != null)
      {
        foreach (var variable in variables)
        {
          context.SetVariable(variable.Key, variable.Value);
        }
      }

      return context;
    }

    private static TriggerResult CreateTriggerResult(
      string triggerName,
      TransitionContext context,
      Transition transition)
    {
      var canTrigger = transition != null && transition.CanMakeTransition(context);

      return new TriggerResult(triggerName, context, canTrigger);
    }

    private Transition GetTransition(string triggerName, IWorkflow workflow)
    {
      return _definition.Transitions
        .SingleOrDefault(t => t.Trigger == triggerName && t.State == workflow.State);
    }

    private TriggerResult CanMakeTransition(
      TransitionContext context,
      string triggerName,
      IWorkflow workflow)
    {
      var transition = GetTransition(triggerName, workflow);
      var triggerResult = CreateTriggerResult(triggerName, context, transition);

      if (transition != null) return triggerResult;

      context.AddError($"Transition for trigger '{triggerName}' not found!");

      return triggerResult;
    }
  }
}
