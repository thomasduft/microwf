using Microsoft.EntityFrameworkCore;

namespace WebApi.Domain
{
  public class DomainContext : DbContext
  {
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    { }

    public DbSet<Holiday> Holidays { get; set; }
  }
}
