using System;
using System.Collections.Generic;
using tomware.MicroWF.Definition;

namespace tomware.MicroWF.Execution
{
  /// <summary>
  /// Provides information about the trigger to be executed.
  /// </summary>
  public class TransitionContext
  {
    private Dictionary<string, WorkflowVariableBase> _variables { get; set; }
    private List<string> _errors;

    /// <summary>
    /// Indicates whether variables are available.
    /// </summary>
    public bool HasVariables
    {
      get { return _variables.Count > 0; }
    }

    /// <summary>
    /// Indicates whether a transition should be aborted.
    /// </summary>
    public bool TransitionAborted { get; private set; }

    /// <summary>
    /// Instance of the workflow
    /// </summary>
    public IWorkflow Workflow { get; private set; }

    /// <summary>
    /// Returns a list of error messages if during triggering some validation failed.
    /// </summary>
    public IEnumerable<string> Errors
    {
      get { return _errors; }
    }

    public TransitionContext(IWorkflow workflow)
    {
      Workflow = workflow;
      _variables = new Dictionary<string, WorkflowVariableBase>();
      _errors = new List<string>();
    }

    /// <summary>
    /// Returns the typed workflow that you expect.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetWorkflow<T>() where T : IWorkflow
    {
      return (T)Workflow;
    }

    /// <summary>
    /// Checks whether a key is available for the variables dictionary.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key)
    {
      if (!HasVariables) return false;

      return _variables.ContainsKey(key);
    }

    /// <summary>
    /// Sets a workflow variable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetVariable<T>(string key, T value) where T : WorkflowVariableBase
    {
      if (_variables.ContainsKey(key))
      {
        _variables[key] = value;
      }
      else
      {
        _variables.Add(key, value);
      }
    }

    /// <summary>
    /// Gets a workflow variable by its key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetVariable<T>(string key) where T : WorkflowVariableBase
    {
      if (!_variables.ContainsKey(key))
        throw new Exception(string.Format("Key '{0}' not found!", key));

      return (T)_variables[key];
    }

    /// <summary>
    /// Adds an error message.
    /// Note: Use it to do validation.
    /// </summary>
    /// <param name="message"></param>
    public void AddError(string message)
    {
      _errors.Add(message);
    }

    /// <summary>
    /// Stops the current transition to be done.
    /// Note: this can only be called before the transition is done.
    /// </summary>
    public void AbortTransition(string reason)
    {
      TransitionAborted = true;
      _errors.Add(reason);
    }
  }
}
