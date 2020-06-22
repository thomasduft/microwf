
using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class WorkItemViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string TriggerName { get; set; }

    [Required]
    public int EntityId { get; set; }

    [Required]
    public string WorkflowType { get; set; }

    public int Retries { get; set; }

    public string Error { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
  }
}