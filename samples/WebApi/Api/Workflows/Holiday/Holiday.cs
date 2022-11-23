using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Holiday
{
  [Table("Holiday")]
  public partial class Holiday : IAssignableWorkflow
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
    public string Requester { get; set; }

    public string Superior { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public List<HolidayMessage> Messages { get; set; } = new List<HolidayMessage>();

    public static Holiday Create(string requester)
    {
      return new Holiday
      {
        Type = HolidayApprovalWorkflow.TYPE,
        State = HolidayApprovalWorkflow.NEW_STATE,
        Assignee = requester,
        Requester = requester
      };
    }

    public void AddMessage(string author, string message)
    {
      this.Messages.Add(new HolidayMessage
      {
        Author = author,
        Message = message,
        Holiday = this
      });
    }
  }

  [Table("HolidayMessage")]
  public class HolidayMessage
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Author { get; set; }

    [Required]
    public string Message { get; set; }

    public int HolidayId { get; set; }

    public Holiday Holiday { get; set; }
  }
}