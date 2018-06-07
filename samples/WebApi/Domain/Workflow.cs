using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain
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

    public DateTime? DueDate { get; set; }

    public WorkflowContext WorkflowContext { get; set; }

    public static Workflow Create(
      int correlationId,
      string type,
      string state,
      string assignee,
      string context = null,
      DateTime? dueDate = null
    )
    {
      return new Workflow
      {
        CorrelationId = correlationId,
        Type = type,
        State = state,
        Assignee = assignee,
        DueDate = dueDate,
        WorkflowContext = WorkflowContext.Create(context)
      };
    }
  }
}
