using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Engine;
using WebApi.Workflows.Issue;

namespace WebApi.Domain
{
  [Table("Issue")]
  public partial class Issue : IEntityWorkflow
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string Assignee { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

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
