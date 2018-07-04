using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public IssueService(
      DomainContext context,
      IWorkflowEngine workflowEngine,
      UserContextService userContext
    )
    {
      this._context = context;
      this._workflowEngine = workflowEngine;
      this._userContext = userContext;
    }

    public async Task<IWorkflowResult<IssueViewModel>> NewAsync()
    {
      var issue = Issue.Create(UserContextService.SYSTEM_USER);
      var triggerParam = new TriggerParam(IssueTrackingWorkflow.ASSIGN_TRIGGER, issue);

      var triggerResult = this._workflowEngine.CanTrigger(triggerParam);

      var info = this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new IssueViewModel();

      var result = new WorkflowResult<Issue, IssueViewModel>(info, issue, viewModel);

      return await Task.FromResult<IWorkflowResult<IssueViewModel>>(result);
    }

    public async Task<int> CreateAsync(IssueViewModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      var issue = Issue.Create(UserContextService.SYSTEM_USER);
      issue.Title = model.Title;
      issue.Description = model.Description;

      this._context.Issues.Add(issue);

      await this._context.SaveChangesAsync();

      return issue.Id;
    }

    public async Task<IWorkflowResult<IssueViewModel>> GetAsync(int id)
    {
      var issue = await this.FindOrCreate(id);

      return ToResult(issue);
    }

    public async Task<IWorkflowResult<NoWorkflowResult>> ProcessAsync(IssueViewModel model)
    {
      var issue = await FindOrCreate(model.Id);

      var triggerParam = new TriggerParam(model.Trigger, issue)
       .AddVariable(IssueViewModel.KEY, model);

      var triggerResult = this._workflowEngine.Trigger(triggerParam);

      var info = this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new NoWorkflowResult(issue.Assignee);

      return new WorkflowResult<Issue, NoWorkflowResult>(info, issue, viewModel);
    }

    public async Task<IEnumerable<Issue>> MyWorkAsync()
    {
      var users = new List<string> { UserContextService.SYSTEM_USER, _userContext.UserName };

      var issues = await this._context.Issues
        .Where(h => users.Contains(h.Assignee))
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
        issue = Issue.Create(UserContextService.SYSTEM_USER);
        this._context.Issues.Add(issue);
      }

      return issue;
    }

    private IWorkflowResult<IssueViewModel> ToResult(Issue issue)
    {
      var info = this._workflowEngine.ToWorkflowTriggerInfo(issue);
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