using System.ComponentModel.DataAnnotations;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Holiday
{
  public class HolidayViewModel : WorkflowVariableBase
  {
    public const string KEY = "HolidayViewModel";

    public int? Id { get; set; }

    [Required]
    public string Requestor { get; set; }

    public string Superior { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string State { get; set; }
  }

  public class ApplyHolidayViewModel : WorkflowVariableBase
  {
    [Required]
    public DateTime From { get; set; }

    [Required]
    public DateTime To { get; set; }

    public string Message { get; set; }

    public ApplyHolidayViewModel()
    {
      var today = SystemTime.Now().Date;

      this.From = today;
      this.To = today.AddDays(1);
    }
  }

  public class ApproveHolidayViewModel : WorkflowVariableBase
  {
    [Required]
    public int Id { get; set; }

    public string Message { get; set; }
  }
}