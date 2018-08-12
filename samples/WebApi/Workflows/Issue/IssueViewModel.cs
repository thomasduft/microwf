using System.ComponentModel.DataAnnotations;
using tomware.Microwf.Core;

namespace WebApi.Workflows.Issue
{
  public class IssueViewModel : WorkflowVariableBase
  {
    public int? Id { get; set; }

    [Required]
    public string Trigger { get; set; } = IssueTrackingWorkflow.ASSIGN_TRIGGER;

    public string Assignee { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
  }
}