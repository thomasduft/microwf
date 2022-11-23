using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace tomware.Microwf.Domain
{
  public sealed class WorkflowCount : BaseSpecification<Workflow>
  {
    public WorkflowCount() : base()
    {
    }
  }

  public sealed class WorkflowInstancesOrderedPaginated : BaseSpecification<Workflow>
  {
    public WorkflowInstancesOrderedPaginated(
      int skip, int take
    ) : base()
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
    ) : base()
    {
      var criterias = GetCriterias(pagingParameters);
      foreach (var criteria in criterias)
      {
        this.AddCriteria(criteria);
      }

      this.ApplyOrderByDescending(w => w.Id);
      this.ApplyPaging(pagingParameters.SkipCount, pagingParameters.PageSize);
      this.ApplyNoTracking();
    }

    private static List<Expression<Func<Workflow, bool>>> GetCriterias(
      WorkflowSearchPagingParameters pagingParameters
    )
    {
      List<Expression<Func<Workflow, bool>>> criterias
        = new List<Expression<Func<Workflow, bool>>>();

      if (pagingParameters.HasType)
      {
        criterias.Add(w => w.Type.ToLower()
          .StartsWith(pagingParameters.Type.ToLower()));
      }

      if (pagingParameters.HasCorrelationId)
      {
        criterias.Add(
          w => w.CorrelationId == pagingParameters.CorrelationId);
      }

      if (pagingParameters.HasAssignee)
      {
        criterias.Add(w => w.Assignee.ToLower()
          .StartsWith(pagingParameters.Assignee.ToLower()));
      }

      return criterias;
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