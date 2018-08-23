using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestDbContext : EngineDbContext
  {
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<LigthtSwitcher> Switchers { get; set; }

    public static DbContextOptions CreateDbContextOptions()
    {
      return new DbContextOptionsBuilder<EngineDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;
    }
  }
}