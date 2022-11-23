using System;
using System.Text.Json.Serialization;

namespace tomware.Microwf.Domain
{
  public partial class WorkflowHistory : EngineEntity
  {
    public DateTime Created { get; set; }

    public string FromState { get; set; }

    public string ToState { get; set; }

    public string UserName { get; set; }

    public int WorkflowId { get; set; }

    [JsonIgnore]
    public Workflow Workflow { get; set; }
  }
}