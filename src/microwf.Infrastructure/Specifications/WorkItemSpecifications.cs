using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class WorkItemDueDateGreaterThanNowCount
    : BaseSpecification<WorkItem>
  {
    public WorkItemDueDateGreaterThanNowCount(DateTime now)
      : base(wi => wi.DueDate > now)
    {
    }
  }

  public sealed class WorkItemDueDateGreaterThanNowOrderedPaginated
  : WorkItemDueDateGreaterThanNowCount
  {
    public WorkItemDueDateGreaterThanNowOrderedPaginated(
      DateTime now,
      int skip,
      int take
    ) : base(now)
    {
      this.ApplyOrderBy(wi => wi.DueDate);
      this.ApplyPaging(skip, take);
      this.ApplyNoTracking();
    }
  }

  public class WorkItemRetryLimitHitCount
    : BaseSpecification<WorkItem>
  {
    public WorkItemRetryLimitHitCount(int retryLimit)
      : base(wi => wi.Retries > retryLimit)
    {
    }
  }

  public sealed class WorkItemRetryLimitHitCountOrderedPaginated
: WorkItemRetryLimitHitCount
  {
    public WorkItemRetryLimitHitCountOrderedPaginated(
      int retryLimit,
      int skip,
      int take
    ) : base(retryLimit)
    {
      this.ApplyOrderBy(wi => wi.DueDate);
      this.ApplyPaging(skip, take);
      this.ApplyNoTracking();
    }
  }

  public sealed class WorkItemFilterForItemsToResume
  : BaseSpecification<WorkItem>
  {
    public WorkItemFilterForItemsToResume(DateTime now, int retryLimit)
      : base(wi => wi.Retries <= retryLimit && wi.DueDate <= now)
    {
    }
  }

  public sealed class WorkItemFilterForExistingItems
    : BaseSpecification<WorkItem>
  {
    public WorkItemFilterForExistingItems(IEnumerable<int> ids)
      : base(wi => ids.Contains(wi.Id))
    {
    }
  }
}