using System.Collections.Generic;

namespace microwf.Execution
{
  public class TriggerParam
  {
    public string TriggerName { get; private set; }
    public IWorkflow Instance { get; private set; }
    public Dictionary<string, WorkflowVariableBase> Variables { get; private set; }
    public bool HasVariables
    {
      get
      {
        return Variables != null;
      }
    }

    public TriggerParam(
      string triggerName,
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null)
    {
      TriggerName = triggerName;
      Instance = instance;

      if (variables != null)
      {
        Variables = variables;
      }

    }
  }
}
