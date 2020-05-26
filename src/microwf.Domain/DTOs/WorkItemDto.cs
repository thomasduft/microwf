using System;

namespace tomware.Microwf.Domain
{
  public class WorkItemDto
  {
    public int Id { get; set; }

    public string TriggerName { get; set; }

    public int EntityId { get; set; }

    public string WorkflowType { get; set; }

    public int Retries { get; set; }

    public string Error { get; set; }

    public DateTime DueDate { get; set; }
  }
}