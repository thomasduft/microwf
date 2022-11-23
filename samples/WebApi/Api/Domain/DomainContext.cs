using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Infrastructure;
using WebApi.Workflows.Holiday;
using WebApi.Workflows.Issue;
using WebApi.Workflows.Stepper;

namespace WebApi.Domain
{
  public class DomainContext : EngineDbContext
  {
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    public DbSet<Issue> Issues { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<Stepper> Steppers { get; set; }
  }
}