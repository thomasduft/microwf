using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class STSDbContext : IdentityDbContext<ApplicationUser>
{
  public STSDbContext(DbContextOptions<STSDbContext> options)
      : base(options) { }

  protected override void OnModelCreating(ModelBuilder builder) => base.OnModelCreating(builder);
}