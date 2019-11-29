using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class EnqueueWorkItemViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Trigger { get; set; }

    public DateTime? DueDate { get; set; }
  }
}