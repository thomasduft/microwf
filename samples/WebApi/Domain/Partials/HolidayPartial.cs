using System.ComponentModel.DataAnnotations;
using tomware.Microwf;
using WebApi.Workflows;

namespace WebApi.Domain
{
  public partial class Holiday : IWorkflow
  {
    [Required]
    public string Type { get; set; }

    [Required]
    public string State { get; set; }

    public static Holiday Create(string requestor)
    {
      return new Holiday
      {
        Type = HolidayApprovalWorkflow.NAME,
        State = HolidayApprovalWorkflow.NEW_STATE,
        Requestor = requestor
      };
    }
  }
}
