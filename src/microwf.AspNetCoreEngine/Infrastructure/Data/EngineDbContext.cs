using Microsoft.EntityFrameworkCore;

namespace tomware.Microwf.Engine
{
  public class EngineDbContext : DbContext
  {
    public EngineDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkItem> WorkItems { get; set; }
  }
}
