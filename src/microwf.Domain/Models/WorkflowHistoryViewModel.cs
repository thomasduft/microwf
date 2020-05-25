using System;

namespace tomware.Microwf.Domain
{
  // TODO: Rename to DTO!
  public class WorkflowHistoryViewModel
  {
    public int Id { get; set; }

    public DateTime Created { get; set; }

    public string FromState { get; set; }

    public string ToState { get; set; }

    public string UserName { get; set; }

    public int WorkflowId { get; set; }
  }
}
