using System;
using System.Linq.Expressions;

namespace tomware.Microwf.Engine
{
  public sealed class WorkflowCount : BaseSpecification<Workflow>
  {
    public WorkflowCount() : base(null)
    {
    }
  }

  public sealed class WorkflowInstancesOrderedPaginated : BaseSpecification<Workflow>
  {
    public WorkflowInstancesOrderedPaginated(
      int skip, int take
    ) : base(PredicateBuilder.True<Workflow>())
    {
      this.ApplyOrderByDescending(w => w.Id);
      this.ApplyPaging(skip, take);
      this.ApplyNoTracking();
    }
  }

  public sealed class WorkflowInstancesFilterAndOrderedPaginated : BaseSpecification<Workflow>
  {
    public WorkflowInstancesFilterAndOrderedPaginated(
      WorkflowSearchPagingParameters pagingParameters
    ) : base(GetWhereClause(pagingParameters))
    {
      this.ApplyOrderByDescending(w => w.Id);
      this.ApplyPaging(pagingParameters.SkipCount, pagingParameters.PageSize);
      this.ApplyNoTracking();
    }

    private static Expression<Func<Workflow, bool>> GetWhereClause(
      WorkflowSearchPagingParameters pagingParameters
    )
    {
      var predicate = PredicateBuilder.True<Workflow>();
      if (pagingParameters.HasType)
      {
        predicate = predicate
          .And(w => w.Type.ToLowerInvariant()
            .StartsWith(pagingParameters.Type.ToLowerInvariant()));
      }
      if (pagingParameters.HasCorrelationId)
      {
        predicate = predicate
          .And(w => w.CorrelationId == pagingParameters.CorrelationId);
      }
      if (pagingParameters.HasAssignee)
      {
        predicate = predicate
          .And(w => w.Assignee.ToLowerInvariant()
            .StartsWith(pagingParameters.Assignee.ToLowerInvariant()));
      }

      return predicate;
    }
  }

  public sealed class WorkflowVariablesForWorkflow : BaseSpecification<Workflow>
  {
    public WorkflowVariablesForWorkflow(int id) : base(w => w.Id == id)
    {
      this.AddInclude(w => w.WorkflowVariables);
      this.ApplyNoTracking();
    }
  }

  public sealed class WorkflowHistoryForWorkflow : BaseSpecification<Workflow>
  {
    public WorkflowHistoryForWorkflow(int id) : base(w => w.Id == id)
    {
      this.AddInclude(w => w.WorkflowHistories);
      this.ApplyNoTracking();
    }
  }

  public sealed class GetWorkflowInstance : BaseSpecification<Workflow>
  {
    public GetWorkflowInstance(string type, int correlationId)
    : base(w => w.Type == type && w.CorrelationId == correlationId)
    {
      this.ApplyNoTracking();
    }
  }

  public sealed class GetWorkflowInstanceHistories : BaseSpecification<Workflow>
  {
    public GetWorkflowInstanceHistories(string type, int correlationId)
    : base(w => w.Type == type && w.CorrelationId == correlationId)
    {
      this.AddInclude(w => w.WorkflowHistories);
      this.ApplyNoTracking();
    }
  }

  public sealed class GetWorkflowInstanceVariables : BaseSpecification<Workflow>
  {
    public GetWorkflowInstanceVariables(string type, int correlationId)
    : base(w => w.Type == type && w.CorrelationId == correlationId)
    {
      this.AddInclude(w => w.WorkflowVariables);
      this.ApplyNoTracking();
    }
  }
}
