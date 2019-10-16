using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public interface IAsyncRepository<T> where T : EngineEntity
  {
    DatabaseFacade Database { get; }

    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync(ISpecification<T> spec);
  }
}
