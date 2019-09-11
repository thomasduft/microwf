using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StepperApi.Domain
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync();
  }

  public class MigrationService : IMigrationService
  {
    private readonly DomainContext context;

    public MigrationService(DomainContext context)
    {
      this.context = context;
    }

    public async Task EnsureMigrationAsync()
    {
      await this.context.Database.MigrateAsync();
    }
  }
}