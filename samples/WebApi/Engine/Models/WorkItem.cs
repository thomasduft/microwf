namespace tomware.Microwf.Engine
{
  public class WorkItem
  {
    public string TriggerName { get; set; }
    public int EntityId { get; set; }
    public string WorkflowType { get; set; }

    public static WorkItem Create(string triggerName, int entityId, string workflowType)
    {
      return new WorkItem
      {
        TriggerName = triggerName,
        EntityId = entityId,
        WorkflowType = workflowType
      };
    }
  }
}
