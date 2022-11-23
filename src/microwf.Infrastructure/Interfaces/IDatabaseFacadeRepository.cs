using Microsoft.EntityFrameworkCore.Infrastructure;

namespace tomware.Microwf.Infrastructure
{
  public interface IDatabaseFacadeRepository
  {
    DatabaseFacade Database { get; }
  }
}