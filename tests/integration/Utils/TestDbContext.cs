using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.TestsCommon.WorkflowDefinitions;

namespace tomware.Microwf.IntegrationTests.Utils
{
  public class TestDbContext : EngineDbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<LightSwitcher> Switchers { get; set; }
  }
}