using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Engine;

namespace WebApi.Domain
{
  public class DomainContext : DbContext
  {
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<Workflow> Workflows { get; set; }
    // public DbSet<WorkflowContext> WorkflowContexts { get; set; }
  }
}
