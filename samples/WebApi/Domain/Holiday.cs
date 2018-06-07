using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Engine;
using WebApi.Workflows.Holiday;

namespace WebApi.Domain
{
  [Table("Holiday")]
  public partial class Holiday : IEntityWorkflow
  {
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string Assignee { get; set; }

    [Required]
    public string Requestor { get; set; }

    public string Superior { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public static Holiday Create(string requestor)
    {
      return new Holiday
      {
        Type = HolidayApprovalWorkflow.TYPE,
        State = HolidayApprovalWorkflow.NEW_STATE,
        Assignee = requestor,
        Requestor = requestor
      };
    }
  }
}
