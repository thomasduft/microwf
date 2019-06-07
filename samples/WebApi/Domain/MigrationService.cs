using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Domain
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync();
  }

  public class MigrationService : IMigrationService
  {
    private readonly DomainContext _context;

    public MigrationService(DomainContext context)
    {
      _context = context;
    }

    public async Task EnsureMigrationAsync()
    {
      await _context.Database.MigrateAsync();
    }
  }
}