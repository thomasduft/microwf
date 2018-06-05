using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
  public partial class Issue
  {
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
  }
}
