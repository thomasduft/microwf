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

  public interface IWorkflowResult<T>
  {
    WorkflowTriggerInfo TriggerInfo { get; set; }

    T ViewModel { get; set; }
  }

  public class WorkflowResult<T> : IWorkflowResult<T>
  {
    public WorkflowTriggerInfo TriggerInfo { get; set; }

    public T ViewModel { get; set; }

    public WorkflowResult(WorkflowTriggerInfo info, T viewModel)
    {
      this.TriggerInfo = info;
      this.ViewModel = viewModel;
    }
  }

  public class AssignableWorkflowViewModel
  {
    public int Id { get; set; }
    public string Type { get; set; }
    public string Assignee { get; set; }
    public string Description { get; set; }
  }
}