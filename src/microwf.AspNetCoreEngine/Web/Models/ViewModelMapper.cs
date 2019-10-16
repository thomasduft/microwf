using System.Collections.Generic;
using System.Linq;

namespace tomware.Microwf.Engine
{
  public static class ViewModelMapper
  {
    public static IEnumerable<WorkItemViewModel> ToWorkItemViewModelList(
      IEnumerable<WorkItem> items
    )
    {
      return items.Select(i => new WorkItemViewModel
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

    public static IEnumerable<WorkflowHistoryViewModel> ToWorkflowHistoryViewModelList(
      IEnumerable<WorkflowHistory> items
    )
    {
      return items.Select(i => new WorkflowHistoryViewModel
      {
        Id = i.Id,
        Created = i.Created,
        FromState = i.FromState,
        ToState = i.ToState,
        UserName = i.UserName,
        WorkflowId = i.WorkflowId
      });
    }

    public static IEnumerable<WorkflowVariableViewModel> ToWorkflowVariableViewModelList(
      IEnumerable<WorkflowVariable> items
    )
    {
      return items.Select(i => new WorkflowVariableViewModel
      {
        Id = i.Id,
        Type = i.Type,
        Content = i.Content,
        WorkflowId = i.WorkflowId
      });
    }
  }
}
