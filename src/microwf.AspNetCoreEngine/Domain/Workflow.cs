using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using tomware.Microwf.Core;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("tomware.Microwf.Tests")]

namespace tomware.Microwf.Engine
{
  [Table("Workflow")]
  public partial class Workflow
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public int CorrelationId { get; set; }

    public string Assignee { get; set; }

    public DateTime Started { get; set; }

    public DateTime? Completed { get; set; }

    public List<WorkflowVariable> WorkflowVariables { get; set; } = new List<WorkflowVariable>();

    public List<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();

    internal static Workflow Create(
      int correlationId,
      string type,
      string state,
      string assignee,
      DateTime? dueDate = null
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

    internal void AddVariable(WorkflowVariableBase variable)
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

    internal void AddHistoryItem(string fromState, string toState, string userName)
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
