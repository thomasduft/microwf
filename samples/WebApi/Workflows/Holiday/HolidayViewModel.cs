using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Workflows.Holiday
{
  public class HolidayViewModel 
  {
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Requestor { get; set; }

    public string Superior { get; set; }
    
    [Required]
    public DateTime From { get; set; }
    
    [Required]
    public DateTime To { get; set; }
  }
}