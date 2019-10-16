using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  [Table("WorkflowVariable")]
  public partial class WorkflowVariable : EngineEntity
  {
    [Required]
    public string Type { get; set; }

    [Required]
    public string Content { get; set; }

    public int WorkflowId { get; set; }

    [JsonIgnore]
    public Workflow Workflow { get; set; }

    internal static WorkflowVariable Create(Workflow workflow, WorkflowVariableBase variable)
    {
      return new WorkflowVariable
      {
        WorkflowId = workflow.Id,
        Workflow = workflow,
        Type = KeyBuilder.ToKey(variable.GetType()),
        Content = JsonConvert.SerializeObject(variable)
      };
    }

    internal void UpdateContent(WorkflowVariableBase variable)
    {
      this.Content = JsonConvert.SerializeObject(variable);
    }
  }
}
