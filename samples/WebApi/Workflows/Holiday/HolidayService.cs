using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Common;
using WebApi.Domain;

namespace WebApi.Workflows.Holiday
{
  public interface IHolidayService
  {
    Task<IWorkflowResult<HolidayViewModel>> GetAsync(int id);

    Task<IWorkflowResult<ApplyHolidayViewModel>> NewAsync();

    Task<IWorkflowResult<NoWorkflowResult>> ApplyAsync(ApplyHolidayViewModel model);

    Task<IWorkflowResult<NoWorkflowResult>> ApproveAsync(ApproveHolidayViewModel model);

    Task<IWorkflowResult<NoWorkflowResult>> RejectAsync(ApproveHolidayViewModel model);

    // TODO: Check for common kind of viewmodel that shows state, short description, id?!
    Task<IEnumerable<Holiday>> MyWorkAsync();
  }

  public class HolidayService : IHolidayService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowEngine _workflowEngine;
    private readonly UserContextService _userContext;

    public HolidayService(
      DomainContext context,
      IWorkflowEngine workflowEngine,
      UserContextService userContext
    )
    {
      this._context = context;
      this._workflowEngine = workflowEngine;
      this._userContext = userContext;
    }

    public async Task<IWorkflowResult<ApplyHolidayViewModel>> NewAsync()
    {
      var holiday = Holiday.Create(_userContext.UserName);
      var triggerParam = new TriggerParam(HolidayApprovalWorkflow.APPLY_TRIGGER, holiday);

      var triggerResult = this._workflowEngine.CanTrigger(triggerParam);

      var info = this.ToWorkflowTriggerInfo(holiday, triggerResult);
      var viewModel = new ApplyHolidayViewModel();

      var result = new WorkflowResult<ApplyHolidayViewModel>(info, viewModel);

      return await Task.FromResult<IWorkflowResult<ApplyHolidayViewModel>>(result);
    }

    public async Task<IWorkflowResult<HolidayViewModel>> GetAsync(int id)
    {
      var holiday = await this._context.Holidays.FindAsync(id);

      return ToResult(holiday);
    }

    public async Task<IWorkflowResult<NoWorkflowResult>> ApplyAsync(ApplyHolidayViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var holiday = await FindOrCreate(null);
      holiday.Superior = "alice";

      var triggerParam = new TriggerParam(HolidayApprovalWorkflow.APPLY_TRIGGER, holiday)
       .AddVariable(ApplyHolidayViewModel.KEY, model);

      var triggerResult = this._workflowEngine.Trigger(triggerParam);

      var info = this.ToWorkflowTriggerInfo(holiday, triggerResult);
      var viewModel = new NoWorkflowResult(holiday.Assignee);

      return new WorkflowResult<NoWorkflowResult>(info, viewModel);
    }

    public async Task<IWorkflowResult<NoWorkflowResult>> ApproveAsync(ApproveHolidayViewModel model)
    {
      var holiday = await FindOrCreate(model.Id);

      var triggerParam = new TriggerParam(HolidayApprovalWorkflow.APPROVE_TRIGGER, holiday)
       .AddVariable(ApproveHolidayViewModel.KEY, model);

      var triggerResult = this._workflowEngine.Trigger(triggerParam);

      var info = this.ToWorkflowTriggerInfo(holiday, triggerResult);
      var viewModel = new NoWorkflowResult(holiday.Assignee);

      return new WorkflowResult<NoWorkflowResult>(info, viewModel);
    }

    public async Task<IWorkflowResult<NoWorkflowResult>> RejectAsync(ApproveHolidayViewModel model)
    {
      var holiday = await FindOrCreate(model.Id);

      var triggerParam = new TriggerParam(HolidayApprovalWorkflow.REJECT_TRIGGER, holiday)
       .AddVariable(ApproveHolidayViewModel.KEY, model);

      var triggerResult = this._workflowEngine.Trigger(triggerParam);

      var info = this.ToWorkflowTriggerInfo(holiday, triggerResult);
      var viewModel = new NoWorkflowResult(holiday.Assignee);

      return new WorkflowResult<NoWorkflowResult>(info, viewModel);
    }

    public async Task<IEnumerable<Holiday>> MyWorkAsync()
    {
      var me = _userContext.UserName;
      var holidays = await this._context.Holidays
        .Where(h => h.Assignee == me)
        .OrderByDescending(h => h.Id)
        .ToListAsync();

      return holidays;
    }

    private IWorkflowResult<HolidayViewModel> ToResult(Holiday holiday)
    {
      var info = this.ToWorkflowTriggerInfo(holiday);
      var viewModel = this.ToViewModel(holiday);

      return new WorkflowResult<HolidayViewModel>(info, viewModel);
    }

    private WorkflowTriggerInfo ToWorkflowTriggerInfo(Holiday holiday, TriggerResult result = null)
    {
      WorkflowTriggerInfo info;
      if (result == null || !result.HasErrors)
      {
        IEnumerable<TriggerResult> triggerResults = this._workflowEngine.GetTriggers(holiday);
        var triggers = triggerResults.Select(x => x.TriggerName);
        info = WorkflowTriggerInfo.createForSuccess(triggers);
      }
      else
      {
        info = WorkflowTriggerInfo.createForError(result.Errors);
      }

      return info;
    }

    private HolidayViewModel ToViewModel(Holiday holiday)
    {
      var viewModel = new HolidayViewModel
      {
        Id = holiday.Id,
        Requestor = holiday.Requester,
        Superior = holiday.Superior,
        From = holiday.From,
        To = holiday.To,
        State = holiday.State
      };

      return viewModel;
    }

    private async Task<Holiday> FindOrCreate(int? id)
    {
      Holiday holiday;

      if (id.HasValue)
      {
        holiday = await this._context.Holidays.FindAsync(id.Value);
      }
      else
      {
        holiday = Holiday.Create(_userContext.UserName);
        this._context.Add(holiday);
      }

      return holiday;
    }
  }
}