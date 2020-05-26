using System.Collections.Generic;
using System.Linq;

namespace tomware.Microwf.Domain
{
  public static class ObjectMapper
  {
    public static IEnumerable<WorkItemDto> ToWorkItemViewModelList(
      IEnumerable<WorkItem> items
    )
    {
      return items.Select(i => new WorkItemDto
      {
        Id = i.Id,
        TriggerName = i.TriggerName,
        EntityId = i.EntityId,
        WorkflowType = i.WorkflowType,
        Retries = i.Retries,
        Error = i.Error,
        DueDate = i.DueDate
      });
    }

    public static IEnumerable<WorkflowHistoryDto> ToWorkflowHistoryViewModelList(
      IEnumerable<WorkflowHistory> items
    )
    {
      return items.Select(i => new WorkflowHistoryDto
      {
        Id = i.Id,
        Created = i.Created,
        FromState = i.FromState,
        ToState = i.ToState,
        UserName = i.UserName,
        WorkflowId = i.WorkflowId
      });
    }

    public static IEnumerable<WorkflowVariableDto> ToWorkflowVariableViewModelList(
      IEnumerable<WorkflowVariable> items
    )
    {
      return items.Select(i => new WorkflowVariableDto
      {
        Id = i.Id,
        Type = i.Type,
        Content = i.Content,
        WorkflowId = i.WorkflowId
      });
    }
  }
}
