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
  }
}
