using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApi.Domain
{
  public interface IEnsureDatabaseService
  {
    Task EnsureSeedData();
  }

  public class EnsureDatabaseService : IEnsureDatabaseService
  {
    private readonly ILogger _logger;
    private readonly DomainContext _context;

    public EnsureDatabaseService(
      ILogger<EnsureDatabaseService> logger,
      DomainContext context
    )
    {
      _logger = logger;
      _context = context;
    }

    public async Task EnsureSeedData()
    {
      _context.Database.Migrate();

      await Task.CompletedTask;
    }
  }
}