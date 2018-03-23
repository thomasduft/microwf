using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public partial class WorkflowContext
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int CorrelationId { get; set; }

    [Required]
    public string Type { get; set; }

    public DateTime? DueDate { get; set; }

    public string Context { get; set; }

    public static WorkflowContext Create(
     int correlationId,
     string type,
     string context = null,
     DateTime? dueDate = null
   )
    {
      return new WorkflowContext
      {
        CorrelationId = correlationId,
        Type = type,
        Context = context,
        DueDate = dueDate ?? null
      };
    }
  }
}
