using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.UnitTests.WorkflowDefinitions;

namespace tomware.Microwf.UnitTests.Utils
{
  public class TestDbContext : EngineDbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<LightSwitcher> Switchers { get; set; }
  }
}