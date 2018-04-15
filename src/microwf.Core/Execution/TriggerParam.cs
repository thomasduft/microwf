using System.Collections.Generic;

namespace tomware.Microwf.Core
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
        return Variables.Count > 0;
      }
    }

    public TriggerParam(
      string triggerName,
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null)
    {
      TriggerName = triggerName;
      Instance = instance;
      Variables = variables != null 
        ? variables
        : new Dictionary<string, WorkflowVariableBase>();
    }
  }
}
