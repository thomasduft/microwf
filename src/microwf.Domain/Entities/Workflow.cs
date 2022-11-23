using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public partial class Workflow : EngineEntity
  {
    public string Type { get; set; }

    public string State { get; set; }

    public int CorrelationId { get; set; }

    public string Assignee { get; set; }

    public DateTime Started { get; set; }

    public DateTime? Completed { get; set; }

    public List<WorkflowVariable> WorkflowVariables { get; set; } = new List<WorkflowVariable>();

    public List<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();

    public static Workflow Create(
      int correlationId,
      string type,
      string state,
      string assignee
    )
    {
      return new Workflow
      {
        Started = SystemTime.Now(),
        CorrelationId = correlationId,
        Type = type,
        State = state,
        Assignee = assignee
      };
    }

    public void AddVariable(WorkflowVariableBase variable)
    {
      var type = KeyBuilder.ToKey(variable.GetType());
      var existing = this.WorkflowVariables.FirstOrDefault(v => v.Type == type);
      if (existing != null)
      {
        existing.UpdateContent(variable);

        return;
      }

      this.WorkflowVariables.Add(WorkflowVariable.Create(this, variable));
    }

    public void AddHistoryItem(string fromState, string toState, string userName)
    {
      this.WorkflowHistories.Add(new WorkflowHistory
      {
        Created = SystemTime.Now(),
        FromState = fromState,
        ToState = toState,
        UserName = userName,
        WorkflowId = this.Id,
        Workflow = this
      });
    }
  }
}