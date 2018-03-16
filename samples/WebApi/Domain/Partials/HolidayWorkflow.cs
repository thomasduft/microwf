using tomware.Microwf;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
  public partial class Holiday : IWorkflow
  {
    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }
  }
}
