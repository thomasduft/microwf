using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tomware.Microwf.Engine
{
  [Table("WorkflowHistory")]
  public partial class WorkflowHistory : EngineEntity
  {
    [Required]
    public DateTime Created { get; set; }

    [Required]
    public string FromState { get; set; }

    [Required]
    public string ToState { get; set; }

    public string UserName { get; set; }

    public int WorkflowId { get; set; }

    [JsonIgnore]
    public Workflow Workflow { get; set; }
  }
}
