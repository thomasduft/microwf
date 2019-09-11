using Microsoft.EntityFrameworkCore;
using StepperApi.Common;
using StepperApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microbus.Core;
using tomware.Microwf.Engine;

namespace StepperApi.Workflows.Stepper
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
    private readonly DomainContext context;
    private readonly IWorkflowEngine workflowEngine;
    private readonly IUserContextService userContext;
    private readonly IMessageBus messageBus;

    public StepperService(
      DomainContext context,
      IWorkflowEngine workflowEngine,
      IUserContextService userContext,
      IMessageBus messageBus
    )
    {
      this.context = context;
      this.workflowEngine = workflowEngine;
      this.userContext = userContext;
      this.messageBus = messageBus;
    }

    public async Task<int> CreateAsync(string name)
    {
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

      var stepper = Stepper.Create(this.userContext.UserName, name);

      this.context.Steppers.Add(stepper);

      await this.context.SaveChangesAsync();

      return stepper.Id;
    }

    public async Task<IWorkflowResult<StepperViewModel>> GetAsync(int id)
    {
      var stepper = await this.Find(id);

      return await this.ToResult(stepper);
    }

    public async Task ProcessAsync(ProcessStepViewModel model)
    {
      var stepper = await this.Find(model.Id);

      await this.messageBus.PublishAsync(WorkItemMessage.Create(
        model.Trigger,
        stepper.Id,
        stepper.Type
        ));
    }

    public async Task<IEnumerable<Stepper>> MyWorkAsync()
    {
      var steppers = await this.context.Steppers
        .Where(s => s.Assignee == this.userContext.UserName)
        .OrderByDescending(s => s.Id)
        .ToListAsync();

      return steppers;
    }

    private async Task<Stepper> Find(int id)
    {
      return await this.context.Steppers.SingleAsync(s => s.Id == id);
    }

    private async Task<IWorkflowResult<StepperViewModel>> ToResult(Stepper stepper)
    {
      var info = await this.workflowEngine.ToWorkflowTriggerInfo(stepper);
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