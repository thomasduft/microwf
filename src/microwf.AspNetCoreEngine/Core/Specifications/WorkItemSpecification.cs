using System;
using System.Linq.Expressions;

namespace tomware.Microwf.Engine
{
  public sealed class WorkItemSpecification : BaseSpecification<WorkItem>
  {
    public WorkItemSpecification(Expression<Func<WorkItem, bool>> criteria) : base(criteria)
    {
    }

  }
}
