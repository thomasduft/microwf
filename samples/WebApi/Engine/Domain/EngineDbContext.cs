using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;

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
