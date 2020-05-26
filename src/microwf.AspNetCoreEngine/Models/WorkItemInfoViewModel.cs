using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class WorkItemInfoViewModel
  {
    [Required]
    public int Id { get; set; }

    public DateTime? DueDate { get; set; }
  }
}