using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  [Table("WorkflowVariable")]
  public partial class WorkflowVariable
  {
    [Key]
    public int Id { get; set; }

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
      Content = JsonConvert.SerializeObject(variable);
    }
  }
}
