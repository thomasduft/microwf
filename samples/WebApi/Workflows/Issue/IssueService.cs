using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tomware.Microbus.Core;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Common;
using WebApi.Domain;

namespace WebApi.Workflows.Issue
{
  public interface IIssueService
  {
    Task<IWorkflowResult<IssueViewModel>> NewAsync();

    Task<int> CreateAsync(IssueViewModel model);

    Task<IWorkflowResult<IssueViewModel>> GetAsync(int id);

    Task<IWorkflowResult<NoWorkflowResult>> ProcessAsync(IssueViewModel model);

    Task<IEnumerable<Issue>> MyWorkAsync();
  }

  public class IssueService : IIssueService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowEngine _workflowEngine;
    private readonly UserContextService _userContext;
    private readonly IMessageBus _messageBus;

    public IssueService(
      DomainContext context,
      IWorkflowEngine workflowEngine,
      UserContextService userContext,
      IMessageBus messageBus
    )
    {
      this._context = context;
      this._workflowEngine = workflowEngine;
      this._userContext = userContext;
      this._messageBus = messageBus;
    }

    public async Task<IWorkflowResult<IssueViewModel>> NewAsync()
    {
      var issue = Issue.Create(_userContext.UserName);
      var triggerParam = new TriggerParam(IssueTrackingWorkflow.ASSIGN_TRIGGER, issue);

      var triggerResult = await this._workflowEngine.CanTriggerAsync(triggerParam);

      var info = await this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new IssueViewModel();

      var result = new WorkflowResult<Issue, IssueViewModel>(info, issue, viewModel);

      return await Task.FromResult<IWorkflowResult<IssueViewModel>>(result);
    }

    public async Task<int> CreateAsync(IssueViewModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      var issue = Issue.Create(_userContext.UserName);
      issue.Title = model.Title;
      issue.Description = model.Description;

      this._context.Issues.Add(issue);

      await this._context.SaveChangesAsync();

      // WorkItem wi = WorkItem.Create(IssueTrackingWorkflow.ASSIGN_TRIGGER, issue.Id, issue.Type);
      // await this._messageBus.PublishAsync(wi);

      return issue.Id;
    }

    public async Task<IWorkflowResult<IssueViewModel>> GetAsync(int id)
    {
      var issue = await this.FindOrCreate(id);

      return await ToResult(issue);
    }

    public async Task<IWorkflowResult<NoWorkflowResult>> ProcessAsync(IssueViewModel model)
    {
      var issue = await FindOrCreate(model.Id);

      var triggerParam = new TriggerParam(model.Trigger, issue)
       .AddVariable(KeyBuilder.ToKey(typeof(IssueViewModel)), model);

      var triggerResult = await this._workflowEngine.TriggerAsync(triggerParam);

      var info = await this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new NoWorkflowResult(issue.Assignee);

      return new WorkflowResult<Issue, NoWorkflowResult>(info, issue, viewModel);
    }

    public async Task<IEnumerable<Issue>> MyWorkAsync()
    {
      var issues = await this._context.Issues
        .Where(h => h.Assignee == _userContext.UserName)
        .OrderByDescending(h => h.Id)
        .ToListAsync();

      return issues;
    }

    private async Task<Issue> FindOrCreate(int? id)
    {
      Issue issue;

      if (id.HasValue)
      {
        issue = await this._context.Issues
          .SingleAsync(_ => _.Id == id.Value);
      }
      else
      {
        issue = Issue.Create(_userContext.UserName);
        this._context.Issues.Add(issue);
      }

      return issue;
    }

    private async Task<IWorkflowResult<IssueViewModel>> ToResult(Issue issue)
    {
      var info = await this._workflowEngine.ToWorkflowTriggerInfo(issue);
      var viewModel = this.ToViewModel(issue);

      return new WorkflowResult<Issue, IssueViewModel>(info, issue, viewModel);
    }

    private IssueViewModel ToViewModel(Issue issue)
    {
      var viewModel = new IssueViewModel
      {
        Id = issue.Id,
        Trigger = string.Empty,
        Assignee = issue.Assignee,
        Title = issue.Title,
        Description = issue.Description
      };

      return viewModel;
    }
  }
}