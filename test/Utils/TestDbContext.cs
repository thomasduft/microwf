using Microsoft.EntityFrameworkCore;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestDbContext : EngineDbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<LightSwitcher> Switchers { get; set; }
  }
}