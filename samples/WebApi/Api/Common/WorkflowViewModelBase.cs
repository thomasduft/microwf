namespace WebApi.Common
{
  public class WorkflowTriggerInfo
  {
    public bool Succeeded { get { return Errors == null; } }

    public IEnumerable<string> Errors { get; set; }

    public IEnumerable<string> Triggers { get; set; }

    public static WorkflowTriggerInfo Success(IEnumerable<string> triggers)
    {
      return new WorkflowTriggerInfo
      {
        Triggers = triggers
      };
    }

    public static WorkflowTriggerInfo Error(IEnumerable<string> errors)
    {
      return new WorkflowTriggerInfo
      {
        Errors = errors
      };
    }
  }

  public interface IWorkflowResult<TViewModel>
  {
    WorkflowTriggerInfo TriggerInfo { get; set; }

    TViewModel ViewModel { get; set; }
  }

  public class WorkflowResult<TEntity, TViewModel> : IWorkflowResult<TViewModel>
  {
    public WorkflowTriggerInfo TriggerInfo { get; set; }

    public TEntity Entity { get; set; }

    public TViewModel ViewModel { get; set; }

    public WorkflowResult(WorkflowTriggerInfo info, TEntity entity, TViewModel viewModel)
    {
      TriggerInfo = info;
      Entity = entity;
      ViewModel = viewModel;
    }
  }

  public class AssigneeWorkflowResult
  {
    public string Assignee { get; set; }
    public string Message { get; set; }

    public AssigneeWorkflowResult() { }

    public AssigneeWorkflowResult(string assignee)
    {
      Assignee = assignee;
    }

    public AssigneeWorkflowResult(string assignee, string message)
    {
      Assignee = assignee;
      Message = message;
    }
  }
}