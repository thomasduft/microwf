using System;

namespace tomware.Microwf.Domain
{
  public class WorkflowHistoryDto
  {
    public int Id { get; set; }

    public DateTime Created { get; set; }

    public string FromState { get; set; }

    public string ToState { get; set; }

    public string UserName { get; set; }

    public int WorkflowId { get; set; }
  }
}