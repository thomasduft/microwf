using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Domain
{
  public interface IAsyncRepository<T> where T : EngineEntity
  {
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync(ISpecification<T> spec);
  }
}