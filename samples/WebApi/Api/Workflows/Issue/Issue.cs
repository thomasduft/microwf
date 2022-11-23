using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Issue
{
  [Table("Issue")]
  public partial class Issue : IAssignableWorkflow
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string Creator { get; set; }

    [Required]
    public string Assignee { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public static Issue Create(string creator)
    {
      return new Issue
      {
        Type = IssueTrackingWorkflow.TYPE,
        State = IssueTrackingWorkflow.OPEN_STATE,
        Creator = creator,
        Assignee = creator
      };
    }
  }
}