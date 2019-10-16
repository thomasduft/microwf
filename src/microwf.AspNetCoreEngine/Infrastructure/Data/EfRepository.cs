using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  public class EfRepository<T> : IAsyncRepository<T> where T : EngineEntity
  {
    protected readonly EngineDbContext DbContext;

    public EfRepository(EngineDbContext dbContext)
    {
      this.DbContext = dbContext;
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
      return await this.DbContext.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
      return await this.DbContext.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
      return await this.ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
      return await this.ApplySpecification(spec).CountAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
      this.DbContext.Set<T>().Add(entity);

      await this.DbContext.SaveChangesAsync();

      return entity;
    }

    public async Task UpdateAsync(T entity)
    {
      this.DbContext.Entry(entity).State = EntityState.Modified;

      await this.DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
      this.DbContext.Set<T>().Remove(entity);

      await this.DbContext.SaveChangesAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
      return SpecificationEvaluator<T>
        .GetQuery(this.DbContext.Set<T>().AsQueryable(), spec);
    }
  }
}
