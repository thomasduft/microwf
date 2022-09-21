using Microsoft.EntityFrameworkCore;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;
using WebApi.Common;
using WebApi.Domain;

namespace WebApi.Workflows.Stepper
{
  public interface IStepperService
  {
    Task<int> CreateAsync(string name);

    Task<IWorkflowResult<StepperViewModel>> GetAsync(int id);

    Task ProcessAsync(ProcessStepViewModel model);

    Task<IEnumerable<Stepper>> MyWorkAsync();
  }

  public class StepperService : IStepperService
  {
    private readonly DomainContext _context;
    private readonly IWorkflowEngineService _workflowEngine;
    private readonly IUserContextService _userContext;
    private readonly IJobQueueService jobQueueService;

    public StepperService(
      DomainContext context,
      IWorkflowEngineService workflowEngine,
      IUserContextService userContext,
      IJobQueueService jobQueueService
    )
    {
      _context = context;
      _workflowEngine = workflowEngine;
      _userContext = userContext;
      this.jobQueueService = jobQueueService;
    }

    public async Task<int> CreateAsync(string name)
    {
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

      var stepper = Stepper.Create(_userContext.UserName, name);

      _context.Steppers.Add(stepper);

      await this._context.SaveChangesAsync();

      return stepper.Id;
    }

    public async Task<IWorkflowResult<StepperViewModel>> GetAsync(int id)
    {
      var stepper = await this.Find(id);

      return await ToResult(stepper);
    }

    public async Task ProcessAsync(ProcessStepViewModel model)
    {
      var stepper = await this.Find(model.Id);

      await this.jobQueueService.Enqueue(WorkItem.Create(
        model.Trigger,
        stepper.Id,
        stepper.Type
        ));
    }

    public async Task<IEnumerable<Stepper>> MyWorkAsync()
    {
      var steppers = await this._context.Steppers
        .Where(s => s.Assignee == _userContext.UserName)
        .OrderByDescending(s => s.Id)
        .ToListAsync();

      return steppers;
    }

    private async Task<Stepper> Find(int id)
    {
      return await this._context.Steppers.SingleAsync(s => s.Id == id);
    }

    private async Task<IWorkflowResult<StepperViewModel>> ToResult(Stepper stepper)
    {
      var info = await this._workflowEngine.ToWorkflowTriggerInfo(stepper);
      var viewModel = this.ToViewModel(stepper);

      return new WorkflowResult<Stepper, StepperViewModel>(info, stepper, viewModel);
    }

    private StepperViewModel ToViewModel(Stepper stepper)
    {
      var viewModel = new StepperViewModel
      {
        Id = stepper.Id,
        Name = stepper.Name
      };

      return viewModel;
    }
  }
}