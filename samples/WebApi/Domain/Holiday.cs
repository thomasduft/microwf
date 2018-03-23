using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
  public partial class Holiday
  {
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Requestor { get; set; }
    public string Superior { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}
