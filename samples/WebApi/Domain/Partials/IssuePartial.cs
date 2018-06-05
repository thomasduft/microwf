using System.ComponentModel.DataAnnotations;
using tomware.Microwf.Engine;
using WebApi.Workflows.Issue;

namespace WebApi.Domain
{
  public partial class Issue : IEntityWorkflow
  {
    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    public string Assignee { get; set; }

    public static Issue Create(string title)
    {
      return new Issue
      {
        Type = IssueTrackingWorkflow.TYPE,
        State = IssueTrackingWorkflow.OPEN_STATE,
        Title = title
      };
    }
  }
}
