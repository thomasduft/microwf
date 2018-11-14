namespace tomware.Microwf.Engine
{
  public class WorkItemMessage
  {
    public string TriggerName { get; set; }

    public int EntityId { get; set; }

    public string WorkflowType { get; set; }

    public static WorkItemMessage Create(string triggerName, int entityId, string workflowType)
    {
      return new WorkItemMessage
      {
        TriggerName = triggerName,
        EntityId = entityId,
        WorkflowType = workflowType
      };
    }
  }
}