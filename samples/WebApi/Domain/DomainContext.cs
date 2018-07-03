using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Engine;
using WebApi.Workflows.Holiday;
using WebApi.Workflows.Issue;

namespace WebApi.Domain
{
  public class DomainContext : DbContext
  {
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    public DbSet<Issue> Issues { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<Workflow> Workflows { get; set; }
  }
}
