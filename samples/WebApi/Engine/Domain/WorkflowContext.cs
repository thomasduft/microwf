using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tomware.Microwf.Engine
{
  [Table("WorkflowContext")]
  public partial class WorkflowContext
  {
    [Key]
    public int Id { get; set; }

    public string Context { get; set; }

    public int WorkflowId { get; set; }

    public Workflow Workflow { get; set; }

    internal static WorkflowContext Create(string context = null)
    {
      return new WorkflowContext
      {
        Context = context
      };
    }
  }
}
