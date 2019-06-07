using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApi.Domain
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync();
  }

  public class MigrationService : IMigrationService
  {
    private readonly ILogger _logger;
    private readonly DomainContext _context;

    public MigrationService(
      ILogger<MigrationService> logger,
      DomainContext context
    )
    {
      _logger = logger;
      _context = context;
    }

    public async Task EnsureMigrationAsync()
    {
      await _context.Database.MigrateAsync();
    }
  }
}