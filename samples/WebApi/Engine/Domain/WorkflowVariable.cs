using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Common;

namespace tomware.Microwf.Engine
{
  [Table("WorkflowVariable")]
  public partial class WorkflowVariable
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Content { get; set; }

    public int WorkflowId { get; set; }

    public Workflow Workflow { get; set; }
  }
}
