using System.ComponentModel.DataAnnotations;
using WebApi.Engine.Workflows;
using WebApi.Workflows.Holiday;

namespace WebApi.Domain
{
  public partial class Holiday : IEntityWorkflow
  {
    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string Assignee { get; set; }

    public static Holiday Create(string requestor)
    {
      return new Holiday
      {
        Type = HolidayApprovalWorkflow.TYPE,
        State = HolidayApprovalWorkflow.NEW_STATE,
        Assignee = requestor,
        Requestor = requestor
      };
    }
  }
}
