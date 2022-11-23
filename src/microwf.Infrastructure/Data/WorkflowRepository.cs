using System;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class WorkflowRepository<TContext>
    : EfRepository<Workflow>, IWorkflowRepository
    where TContext : EngineDbContext
  {
    public WorkflowRepository(TContext dbContext) : base(dbContext)
    {
    }

    public void AddAutoTrigger(AutoTrigger autoTrigger, IWorkflowInstanceEntity entity)
    {
      var workItem = WorkItem.Create(
        autoTrigger.Trigger,
        entity.Id,
        entity.Type,
        autoTrigger.DueDate
      );

      this.DbContext.WorkItems.Add(workItem);
    }

    public IWorkflow Find(int id, Type type)
    {
      return (IWorkflow)this.DbContext.Find(type, id);
    }

    public async Task ApplyChangesAsync()
    {
      await this.DbContext.SaveChangesAsync();
    }
  }
}