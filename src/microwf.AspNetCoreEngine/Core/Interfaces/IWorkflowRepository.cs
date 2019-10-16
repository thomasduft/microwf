using System;
using System.Threading.Tasks;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowRepository : IAsyncRepository<Workflow>
  {
    IWorkflow Find(int id, Type type);

    Task ApplyChangesAsync();

    void AddWorkItemEntry(AutoTrigger autoTrigger, IWorkflowInstanceEntity entity);
  }
}
