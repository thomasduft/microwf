using System;
using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestDbContext
  {
    public static DbContextOptions CreateDbContextOptions()
    {
      return new DbContextOptionsBuilder<EngineDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;
    }
  }
}