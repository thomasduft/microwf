using System;

namespace tomware.Microwf.Domain
{
  // TODO: Rename to DTO!
  public class WorkflowViewModel
  {
    public int Id { get; set; }
    public int CorrelationId { get; set; }
    public string Type { get; set; }
    public string State { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Assignee { get; set; }
    public string Route { get; set; }
    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; }
  }
}