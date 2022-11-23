using System;

namespace tomware.Microwf.Domain
{
  public partial class WorkItem : EngineEntity
  {
    public static int WORKITEM_RETRIES = 3;

    public string TriggerName { get; set; }

    public int EntityId { get; set; }

    public string WorkflowType { get; set; }

    public int Retries { get; set; }

    public string Error { get; set; }

    public DateTime DueDate { get; set; }

    public static WorkItem Create(
      string triggerName,
      int entityId,
      string workflowType,
      DateTime? dueDate = null
    )
    {
      return new WorkItem
      {
        TriggerName = triggerName,
        EntityId = entityId,
        WorkflowType = workflowType,
        Retries = 0,
        DueDate = dueDate ?? SystemTime.Now()
      };
    }
  }
}