using microwf.Definition;
using System.Collections.Generic;

namespace microwf.Execution
{
  public class TriggerParam
  {
    public string TriggerName { get; private set; }
    public IWorkflow Workflow { get; private set; }
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
      IWorkflow workflow,
      Dictionary<string, WorkflowVariableBase> variables = null)
    {
      TriggerName = triggerName;
      Workflow = workflow;

      if (variables != null)
      {
        Variables = variables;
      }
    }
  }
}
