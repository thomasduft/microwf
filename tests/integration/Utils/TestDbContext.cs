using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;

namespace tomware.Microwf.Tests.Integration.Utils
{
  public class TestDbContext : EngineDbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<LightSwitcher> Switchers { get; set; }
  }
}