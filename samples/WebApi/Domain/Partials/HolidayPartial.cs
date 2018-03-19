using System.ComponentModel.DataAnnotations;
using tomware.Microwf.Core;
using WebApi.Workflows.Holiday;

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
        Type = HolidayApprovalWorkflow.TYPE,
        State = HolidayApprovalWorkflow.NEW_STATE,
        Requestor = requestor
      };
    }
  }
}
