using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class EfRepository<TEntity> : IAsyncRepository<TEntity>, IDatabaseFacadeRepository
    where TEntity : EngineEntity
  {
    protected readonly EngineDbContext DbContext;

    public DatabaseFacade Database => this.DbContext.Database;

    public EfRepository(EngineDbContext dbContext)
    {
      this.DbContext = dbContext;
    }

    public virtual async Task<TEntity> GetByIdAsync(int id)
    {
      return await this.DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
    {
      return await this.ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
      return await this.ApplySpecification(spec).CountAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      this.DbContext.Set<TEntity>().Add(entity);

      await this.DbContext.SaveChangesAsync();

      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      this.DbContext.Entry(entity).State = EntityState.Modified;

      await this.DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
      this.DbContext.Set<TEntity>().Remove(entity);

      await this.DbContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
      return SpecificationEvaluator<TEntity>
        .GetQuery(this.DbContext.Set<TEntity>().AsQueryable(), spec);
    }
  }
}