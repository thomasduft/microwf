using System.Collections.Generic;

namespace WebApi.Models
{
  public interface IWorkflowResult<T>
  {
    IEnumerable<string> Triggers { get; set; }

    T ViewModel { get; set; }
  }

  public class WorkflowResult<T> : IWorkflowResult<T>
  {
    public IEnumerable<string> Triggers { get; set; }

    public T ViewModel { get; set; }

    public WorkflowResult(IEnumerable<string> triggers, T viewModel)
    {
      Triggers = triggers;
      ViewModel = viewModel;
    }
  }
}