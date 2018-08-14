using System.Collections.Generic;

namespace tomware.Microwf.Engine
{
  public class WorkflowTriggerInfo
  {
    public bool Succeeded { get { return this.Errors == null; } }

    public IEnumerable<string> Errors { get; set; }

    public IEnumerable<string> Triggers { get; set; }

    public static WorkflowTriggerInfo createForSuccess(IEnumerable<string> triggers)
    {
      var info = new WorkflowTriggerInfo();
      info.Triggers = triggers;

      return info;
    }

    public static WorkflowTriggerInfo createForError(IEnumerable<string> errors)
    {
      var info = new WorkflowTriggerInfo();
      info.Errors = errors;

      return info;
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
      this.TriggerInfo = info;
      this.Entity = entity;
      this.ViewModel = viewModel;
    }
  }

  public class AssigneeWorkflowResult
  {
    public string Assignee { get; set; }
    public string Message { get; set; }

    public AssigneeWorkflowResult() { }

    public AssigneeWorkflowResult(string assignee)
    {
      this.Assignee = assignee;
    }

    public AssigneeWorkflowResult(string assignee, string message)
    {
      this.Assignee = assignee;
      this.Message = message;
    }
  }
}