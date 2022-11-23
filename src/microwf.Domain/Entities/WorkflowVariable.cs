using System.Text.Json;
using System.Text.Json.Serialization;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public partial class WorkflowVariable : EngineEntity
  {
    public string Type { get; set; }

    public string Content { get; set; }

    public int WorkflowId { get; set; }

    [JsonIgnore]
    public Workflow Workflow { get; set; }

    public static WorkflowVariable Create(Workflow workflow, WorkflowVariableBase variable)
    {
      return new WorkflowVariable
      {
        WorkflowId = workflow.Id,
        Workflow = workflow,
        Type = KeyBuilder.ToKey(variable.GetType()),
        Content = JsonSerializer.Serialize(variable, variable.GetType())
      };
    }

    public static WorkflowVariableBase ConvertContent(WorkflowVariable workflowVariable)
    {
      return (WorkflowVariableBase)JsonSerializer.Deserialize(
        workflowVariable.Content,
        KeyBuilder.FromKey(workflowVariable.Type)
      );
    }

    internal void UpdateContent(WorkflowVariableBase variable)
    {
      this.Content = JsonSerializer.Serialize(variable, variable.GetType());
    }
  }
}