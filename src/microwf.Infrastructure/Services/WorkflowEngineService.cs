using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public class WorkflowEngineService : IWorkflowEngineService
  {
    private readonly IWorkflowRepository repository;
    private readonly ILogger<WorkflowEngineService> logger;
    private readonly IWorkflowDefinitionProvider workflowDefinitionProvider;
    private readonly IUserContextService userContext;

    public WorkflowEngineService(
      IWorkflowRepository repository,
      ILogger<WorkflowEngineService> logger,
      IWorkflowDefinitionProvider workflowDefinitionProvider,
      IUserContextService userContext
    )
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
      this.logger = logger;
      this.workflowDefinitionProvider = workflowDefinitionProvider;
      this.userContext = userContext;
    }

    public async Task<TriggerResult> CanTriggerAsync(TriggerParam param)
    {
      if (param == null) throw new ArgumentNullException(nameof(param));

      var execution = this.GetExecution(param.Instance.Type);

      return await Task.FromResult(execution.CanTrigger(param));
    }

    public async Task<IEnumerable<TriggerResult>> GetTriggersAsync(
      IWorkflow instance,
      Dictionary<string, WorkflowVariableBase> variables = null
    )
    {
      if (instance == null) throw new ArgumentNullException(nameof(instance));

      var execution = this.GetExecution(instance.Type);

      return await Task.FromResult(execution.GetTriggers(instance, variables));
    }

    public async Task<TriggerResult> TriggerAsync(TriggerParam param)
    {
      if (param == null) throw new ArgumentNullException(nameof(param));

      this.logger.LogTrace(
        "Trigger transition for instance {@Instance}",
        param.Instance
      );

      var result = param.Instance is IWorkflowInstanceEntity
        ? await this.TriggerForPersistingInstance(param)
        : this.TriggerForNonPersistingInstance(param);

      if (result.IsAborted)
      {
        this.logger.LogInformation(
          "Transition for instance {@Instance} aborted! Aborting reason: {@Reason}",
          param.Instance,
          result.Errors
        );
      }
      else
      {
        this.logger.LogTrace(
          "Transition for instance {@Instance} successfully triggered",
          param.Instance
        );
      }

      return result;
    }

    public IWorkflow Find(int id, Type type)
    {
      return this.repository.Find(id, type);
    }

    private WorkflowExecution GetExecution(string type)
    {
      var definition = this.workflowDefinitionProvider.GetWorkflowDefinition(type);

      return new WorkflowExecution(definition);
    }

    private TriggerResult TriggerForNonPersistingInstance(TriggerParam param)
    {
      var execution = this.GetExecution(param.Instance.Type);

      return execution.Trigger(param);
    }

    private async Task<TriggerResult> TriggerForPersistingInstance(TriggerParam param)
    {
      TriggerResult result;

      var dbFacadeRepository = this.repository as IDatabaseFacadeRepository;
      if (dbFacadeRepository == null)
      {
        var msg = "Please register a custom IWorkflowEngineService - implementation!"
                + Environment.NewLine + "The default implementation relies on a "
                + "transactional database that exposes the DatabaseFacade property "
                + "in the Microsoft.EntityFrameworkCore.Infrastructure namespace.";
        throw new InvalidOperationException(msg);
      }

      using (var transaction = dbFacadeRepository.Database.BeginTransaction())
      {
        try
        {
          Workflow workflow = null;
          var execution = this.GetExecution(param.Instance.Type);
          var entity = param.Instance as IWorkflowInstanceEntity;

          await this.repository.ApplyChangesAsync(); // so entity id gets resolved!

          workflow = await this.FindOrCreate(
            entity.Id,
            param.Instance.Type,
            param.Instance.State
          );

          this.EnsureWorkflowVariables(workflow, param);

          result = execution.Trigger(param);
          if (!result.IsAborted)
          {
            await this.PersistWorkflow(workflow, param, result);

            await this.repository.ApplyChangesAsync();

            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();

          this.logger.LogError(
            "Trigger with TriggerParameter: {@TriggerParameter} failed! {Exception}",
            param,
            ex
          );

          var transitionContext = new TransitionContext(param.Instance);
          transitionContext.AddError(ex.ToString());

          result = new TriggerResult(
            param.TriggerName,
            transitionContext,
            false
          );
        }
      }

      return result;
    }

    private async Task<Workflow> FindOrCreate(
      int id,
      string type,
      string state,
      string assignee = null
    )
    {
      var list = await this.repository.ListAsync(new GetWorkflowInstanceVariables(type, id));
      var workflow = list.FirstOrDefault();
      if (workflow == null)
      {
        workflow = Workflow.Create(id, type, state, assignee);
        await this.repository.AddAsync(workflow);
      }

      return workflow;
    }

    private void EnsureWorkflowVariables(Workflow workflow, TriggerParam param)
    {
      if (workflow.WorkflowVariables.Count == 0) return;

      foreach (var workflowVariable in workflow.WorkflowVariables)
      {
        var variable = WorkflowVariable.ConvertContent(workflowVariable);
        if (variable is WorkflowVariableBase)
        {
          var key = workflowVariable.Type;
          if (param.Variables.ContainsKey(key))
          {
            param.Variables[key] = variable as WorkflowVariableBase;
          }
          else
          {
            param.Variables.Add(key, variable as WorkflowVariableBase);
          }
        }
      }
    }

    private async Task PersistWorkflow(
      Workflow workflow,
      TriggerParam param,
      TriggerResult result
    )
    {
      if (workflow == null) throw new ArgumentNullException(nameof(workflow));

      // persisting workflow variables
      if (param.HasVariables)
      {
        foreach (var v in param.Variables)
        {
          var variable = workflow.WorkflowVariables
            .FirstOrDefault(variables => variables.Type == v.Key);

          if (variable != null)
          {
            variable.Content = JsonSerializer.Serialize(v.Value);
          }
          else
          {
            workflow.AddVariable(v.Value);
          }
        }
      }

      // keeping workflow entity nsync
      var entity = param.Instance as IWorkflowInstanceEntity;
      var assignableEntity = param.Instance as IAssignableWorkflow;
      if (entity != null)
      {
        workflow.Type = entity.Type;
        workflow.Assignee = assignableEntity.Assignee;

        workflow.AddHistoryItem(workflow.State, entity.State, this.userContext.UserName);
        workflow.State = entity.State;
      }

      // treating AutoTrigger
      if (result.HasAutoTrigger)
      {
        this.repository.AddAutoTrigger(result.AutoTrigger, entity);
      }

      if (await this.WorkflowIsCompleted(param))
      {
        workflow.Completed = SystemTime.Now();
      }
    }

    private async Task<bool> WorkflowIsCompleted(TriggerParam triggerParam)
    {
      var triggerResults
        = await this.GetTriggersAsync(triggerParam.Instance, triggerParam.Variables);

      return triggerResults.All(r => !r.CanTrigger);
    }
  }
}