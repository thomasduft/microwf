using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Engine;
using StepperApi.Workflows.Stepper;

namespace StepperApi.Domain
{
  public class DomainContext : EngineDbContext
  {
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    public DbSet<Stepper> Steppers { get; set; }
  }
}
