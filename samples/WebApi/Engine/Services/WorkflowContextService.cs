using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Domain;

namespace tomware.Microwf.Engine
{
  public interface IWorkflowContextService
  {
    WorkflowContext FindOrCreate(int id, TriggerParam triggerParam);
    
    void Update(
      int id,
      Dictionary<string, WorkflowVariableBase> variables = null,
      DateTime? dueDate = null
      );
  }

  public class WorkflowContextService : IWorkflowContextService
  {
    private readonly DomainContext _context;

    public WorkflowContextService(DomainContext context)    {
      this._context = context;
    }

    public WorkflowContext FindOrCreate(int id, TriggerParam triggerParam)
    {
      var ctx = GetById(id);
      if (ctx == null)
      {
        ctx = WorkflowContext.Create(id, triggerParam.Instance.Type);
        this._context.Add(ctx);
      }

      return ctx;
    }

    public void Update(
      int id,
      Dictionary<string, WorkflowVariableBase> variables = null,
      DateTime? dueDate = null
    )
    {
      string context = null;
      if (variables != null) context = JsonConvert.SerializeObject(variables);

      WorkflowContext workflowContext = GetById(id);
      workflowContext.Context = context;
      workflowContext.DueDate = dueDate;

      this._context.Update(workflowContext);
    }

    private WorkflowContext GetById(int id)
    {
      return this._context.Workflows.SingleOrDefault(w => w.CorrelationId == id);
    }
  }
}
