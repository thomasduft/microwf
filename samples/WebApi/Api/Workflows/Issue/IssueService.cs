using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using WebApi.Common;
using WebApi.Domain;

namespace WebApi.Workflows.Issue
{
  public interface IIssueService
  {
    Task<IEnumerable<string>> GetAssigneesAsync();

    Task<IWorkflowResult<IssueViewModel>> NewAsync();

    Task<int> CreateAsync(IssueViewModel model);

    Task<IWorkflowResult<IssueViewModel>> GetAsync(int id);

    Task<IWorkflowResult<AssigneeWorkflowResult>> ProcessAsync(IssueViewModel model);

    Task<IEnumerable<Issue>> MyWorkAsync();
  }

  public class IssueService : IIssueService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowEngineService _workflowEngine;
    private readonly IUserContextService _userContext;

    public IssueService(
      DomainContext context,
      IWorkflowEngineService workflowEngine,
      IUserContextService userContext
    )
    {
      this._context = context;
      this._workflowEngine = workflowEngine;
      this._userContext = userContext;
    }

    public async Task<IEnumerable<string>> GetAssigneesAsync()
    {
      return await Task.FromResult(new string[] { "Alice", "Admin" });
    }

    public async Task<IWorkflowResult<IssueViewModel>> NewAsync()
    {
      var issue = Issue.Create(_userContext.UserName);
      var triggerParam = new TriggerParam(IssueTrackingWorkflow.ASSIGN_TRIGGER, issue);

      var triggerResult = await this._workflowEngine.CanTriggerAsync(triggerParam);

      var info = await this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new IssueViewModel();
      viewModel.Assignee = _userContext.UserName;

      var result = new WorkflowResult<Issue, IssueViewModel>(info, issue, viewModel);

      return await Task.FromResult<IWorkflowResult<IssueViewModel>>(result);
    }

    public async Task<int> CreateAsync(IssueViewModel model)
    {
      if (model == null) throw new ArgumentNullException("model");

      var issue = Issue.Create(_userContext.UserName);
      issue.Title = model.Title;
      issue.Description = model.Description;
      if (!string.IsNullOrWhiteSpace(model.Assignee)) issue.Assignee = model.Assignee;

      this._context.Issues.Add(issue);

      await this._context.SaveChangesAsync();

      // await this._messageBus.PublishAsync(WorkItemMessage.Create(
      //   IssueTrackingWorkflow.ASSIGN_TRIGGER,
      //   issue.Id,
      //   issue.Type
      // ));

      return issue.Id;
    }

    public async Task<IWorkflowResult<IssueViewModel>> GetAsync(int id)
    {
      var issue = await this.FindOrCreate(id);

      return await ToResult(issue);
    }

    public async Task<IWorkflowResult<AssigneeWorkflowResult>> ProcessAsync(IssueViewModel model)
    {
      var issue = await FindOrCreate(model.Id);

      var triggerParam = new TriggerParam(model.Trigger, issue)
       .AddVariableWithKey<IssueViewModel>(model);

      var triggerResult = await this._workflowEngine.TriggerAsync(triggerParam);

      var info = await this._workflowEngine.ToWorkflowTriggerInfo(issue, triggerResult);
      var viewModel = new AssigneeWorkflowResult(issue.Assignee);

      return new WorkflowResult<Issue, AssigneeWorkflowResult>(info, issue, viewModel);
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
          .SingleAsync(i => i.Id == id.Value);
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