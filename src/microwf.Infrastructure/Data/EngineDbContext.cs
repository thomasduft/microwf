using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure.Configuration;

namespace tomware.Microwf.Infrastructure
{
  public class EngineDbContext : DbContext
  {
    public EngineDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkItem> WorkItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.ApplyConfiguration(new WorkflowEntityConfiguration());
      builder.ApplyConfiguration(new WorkflowHistoryEntityConfiguration());
      builder.ApplyConfiguration(new WorkflowVariableEntityConfiguration());
      builder.ApplyConfiguration(new WorkItemEntityConfiguration());
    }
  }
}