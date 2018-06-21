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

    Task<IWorkflowResult<HolidayViewModel>> NewAsync();

    Task<IWorkflowResult<HolidayViewModel>> ApplyAsync(HolidayViewModel model);

    Task<IWorkflowResult<HolidayViewModel>> ApproveAsync(HolidayViewModel model);

    Task<IWorkflowResult<HolidayViewModel>> RejectAsync(HolidayViewModel model);

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

    public async Task<IWorkflowResult<HolidayViewModel>> GetAsync(int id)
    {
      var holiday = await this._context.Holidays.FindAsync(id);

      return ToResult(holiday);
    }

    public async Task<IWorkflowResult<HolidayViewModel>> NewAsync()
    {
      var holiday = Holiday.Create(_userContext.UserName);

      this._context.Add(holiday);
      await this._context.SaveChangesAsync();

      return ToResult(holiday);
    }

    public async Task<IWorkflowResult<HolidayViewModel>> ApplyAsync(HolidayViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      return await Trigger(HolidayApprovalWorkflow.APPLY_TRIGGER, model);
    }

    public async Task<IWorkflowResult<HolidayViewModel>> ApproveAsync(HolidayViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      return await Trigger(HolidayApprovalWorkflow.APPROVE_TRIGGER, model);
    }

    public async Task<IWorkflowResult<HolidayViewModel>> RejectAsync(HolidayViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      return await Trigger(HolidayApprovalWorkflow.REJECT_TRIGGER, model);
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
      IEnumerable<TriggerResult> result = this._workflowEngine.GetTriggers(holiday);
      var triggers = result.Select(x => x.TriggerName);

      var viewModel = new HolidayViewModel
      {
        Id = holiday.Id,
        Requestor = holiday.Requestor,
        Superior = holiday.Superior,
        From = holiday.From,
        To = holiday.To
      };

      return new WorkflowResult<HolidayViewModel>(triggers, viewModel);
    }

    private async Task<IWorkflowResult<HolidayViewModel>> Trigger(
      string trigger,
      HolidayViewModel model
    )
    {
      var holiday = await this._context.Holidays.FindAsync(model.Id);
      holiday.Superior = model.Superior;
      holiday.From = model.From;
      holiday.To = model.To;

      var triggerParam = new TriggerParam(trigger, holiday)
        .AddVariable(HolidayViewModel.KEY, model);
        
      this._workflowEngine.Trigger(triggerParam);

      return ToResult(holiday);
    }
  }
}