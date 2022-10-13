using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tomware.Microwf.Domain;

namespace WebApi.Workflows.Stepper
{
  [Table("Stepper")]
  public partial class Stepper : IAssignableWorkflow
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
    public string Creator { get; set; }

    [Required]
    public string Name { get; set; }

    public static Stepper Create(string creator, string name)
    {
      return new Stepper
      {
        Type = StepperWorkflow.TYPE,
        State = StepperWorkflow.NEW_STATE,
        Creator = creator,
        Assignee = creator,
        Name = name
      };
    }
  }
}