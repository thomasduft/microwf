using System;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public interface IWorkflowRepository : IAsyncRepository<Workflow>
  {
    IWorkflow Find(int id, Type type);

    Task ApplyChangesAsync();

    void AddAutoTrigger(AutoTrigger autoTrigger, IWorkflowInstanceEntity entity);
  }
}