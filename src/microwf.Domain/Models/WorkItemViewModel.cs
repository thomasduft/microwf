using System;

namespace tomware.Microwf.Domain
{
  // TODO: Rename to DTO!
  public class WorkItemViewModel
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