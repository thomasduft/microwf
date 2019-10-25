using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class WorkflowTriggerViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Trigger { get; set; }
  }
}