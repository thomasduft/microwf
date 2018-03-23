using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Domain;

namespace tomware.Microwf.Engine
{
  public class WorkflowContextInfo
  {
    public IWorkflow Workflow { get; set; }
    public WorkflowContext Context { get; set; }
  }

  public interface IWorkflowContextService
  {
    WorkflowContextInfo FindOrCreate(int id, TriggerParam triggerParam);
    void Save(
      int id,
      IWorkflow workflow,
      DateTime? dueDate = null,
      Dictionary<string, WorkflowVariableBase> variables = null);
  }

  public class WorkflowContextService : IWorkflowContextService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowDefinitionProvider _workflowDefinitionProvider;

    public WorkflowContextService(
      DomainContext context,
      IWorkflowDefinitionProvider workflowDefinitionProvider
    )
    {
      this._context = context;
      this._workflowDefinitionProvider = workflowDefinitionProvider;
    }

    public WorkflowContextInfo FindOrCreate(int id, TriggerParam triggerParam)
    {
      var ctx = GetById(id);
      if (ctx == null)
      {
        ctx = WorkflowContext.Create(id, triggerParam.Instance.Type);
        this._context.Add(ctx);
      }

      var definition = this._workflowDefinitionProvider.GetWorkflowDefinition(ctx.Type);
      var type = ((EntityWorkflowDefinitionBase)definition).EntityType;

      return new WorkflowContextInfo
      {
        Workflow = triggerParam.Instance,
        Context = ctx
      };
    }

    public void Save(
      int id,
      IWorkflow workflow,
      DateTime? dueDate = null,
      Dictionary<string, WorkflowVariableBase> variables = null
    )
    {
      if (workflow == null) throw new ArgumentNullException(nameof(workflow));

      string context = null;
      if (variables != null)
      {
        context = JsonConvert.SerializeObject(variables);
      }

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
