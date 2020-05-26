using System.ComponentModel.DataAnnotations;

namespace tomware.Microwf.Engine
{
  public class WorkflowVariableViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Content { get; set; }

    public int WorkflowId { get; set; }
  }
}