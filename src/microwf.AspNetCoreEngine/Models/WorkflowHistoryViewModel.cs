using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class WorkflowHistoryViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime Created { get; set; }

    [Required]
    public string FromState { get; set; }

    [Required]
    public string ToState { get; set; }

    public string UserName { get; set; }

    public int WorkflowId { get; set; }
  }
}