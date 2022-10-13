using Microsoft.EntityFrameworkCore;

namespace WebApi.Domain
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync(CancellationToken cancellationToken);
  }

  public class MigrationService : IMigrationService
  {
    private readonly DomainContext _context;

    public MigrationService(DomainContext context)
    {
      _context = context;
    }

    public async Task EnsureMigrationAsync(CancellationToken cancellationToken)
    {
      await _context.Database.MigrateAsync();
    }
  }
}