using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using tomware.Microwf.Tests.Integration.Utils;
using Xunit;

namespace tomware.Microwf.Tests.Integration.Infrastructure
{
  public class WorkflowEngineServiceTest
  {
    public TestDbContext Context { get; set; }
    public IWorkflowDefinitionProvider WorkflowDefinitionProvider { get; set; }

    public IWorkflowEngineService WorkflowEngineService { get; set; }

    public WorkflowEngineServiceTest()
    {
      this.Initialize();
    }

    private void Initialize()
    {
      var diHelper = new DITestHelper();
      diHelper.AddTestDbContext();
      diHelper.Services.AddScoped<IWorkflowDefinitionProvider, SimpleWorkflowDefinitionProvider>();
      diHelper.Services.AddTransient<IUserWorkflowMappingService, TestUserWorkflowMappingService>(fact =>
      {
        return new TestUserWorkflowMappingService();
      });
      diHelper.Services.AddTransient<IWorkflowDefinitionDtoCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService>();
      diHelper.Services.AddTransient<IWorkflowRepository, WorkflowRepository<TestDbContext>>();
      diHelper.Services.AddTransient<IWorkflowEngineService, WorkflowEngineService>();
      var serviceProvider = diHelper.Build();

      this.Context = serviceProvider.GetRequiredService<TestDbContext>();
      this.WorkflowDefinitionProvider = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new OnOffWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new EntityOnOffWorkflow());

      this.WorkflowEngineService = serviceProvider.GetRequiredService<IWorkflowEngineService>();
    }

    [Fact]
    public async Task WorkflowEngineService_CanTriggerAsync_CanTriggerWorkflow()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.CanTriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.True(triggerResult.CanTrigger);
    }

    [Fact]
    public async Task WorkflowEngineService_CanTriggerAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      await Assert.ThrowsAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.CanTriggerAsync(null));
    }

    [Fact]
    public async Task WorkflowEngineService_GetTriggersAsync_ReturnsPossibleTriggers()
    {
      // Arrange
      var instance = new Switcher();

      // Act
      var results = await this.WorkflowEngineService.GetTriggersAsync(instance);

      // Assert
      Assert.NotNull(results);
      Assert.Single(results);
      Assert.Equal("SwitchOn", results.First().TriggerName);
    }

    [Fact]
    public async Task WorkflowEngineService_GetTriggersAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Act
      await Assert.ThrowsAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.GetTriggersAsync(null));
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsync_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.False(triggerResult.HasErrors);
      Assert.Equal(instance.State, triggerResult.CurrentState);
      Assert.Equal("On", triggerResult.CurrentState);
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Act
      await Assert.ThrowsAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.TriggerAsync(null));
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsyncWithEntityWorkflowInstance_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.False(triggerResult.HasErrors);
      Assert.Equal(instance.State, triggerResult.CurrentState);
      Assert.Equal("On", triggerResult.CurrentState);

      Assert.Equal(1, this.Context.Workflows.Count());
      Assert.Empty(this.Context.Workflows.First().WorkflowVariables);
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsyncWithEntityWorkflowInstanceAndNewWorkflowVariable_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      var workfowVariable = new LightSwitcherWorkflowVariable { CanSwitch = true };
      var param = new TriggerParam("SwitchOn", instance)
        .AddVariableWithKey<LightSwitcherWorkflowVariable>(workfowVariable);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.False(triggerResult.HasErrors);
      Assert.Equal(instance.State, triggerResult.CurrentState);
      Assert.Equal("On", triggerResult.CurrentState);

      Assert.Equal(1, this.Context.Workflows.Count());
      Assert.Single(this.Context.Workflows.First().WorkflowVariables);
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsyncWithEntityWorkflowInstanceAndExistingWorkflowVariable_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      this.Context.Switchers.Add(instance);

      var workflow = Workflow.Create(instance.Id, instance.Type, instance.State, "tester");
      workflow.AddVariable(new LightSwitcherWorkflowVariable { CanSwitch = true });

      this.Context.Workflows.Add(workflow);
      await this.Context.SaveChangesAsync();

      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.False(triggerResult.HasErrors);
      Assert.Equal(instance.State, triggerResult.CurrentState);
      Assert.Equal("On", triggerResult.CurrentState);

      Assert.True(param.HasVariables);

      Assert.Single(workflow.WorkflowHistories);
    }

    [Fact]
    public async Task WorkflowEngineService_TriggerAsyncWithEntityWorkflowInstanceAndSameWorkflowVariable_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      this.Context.Switchers.Add(instance);

      var workflow = Workflow.Create(instance.Id, instance.Type, instance.State, "tester");
      var variable = new LightSwitcherWorkflowVariable { CanSwitch = true };
      workflow.AddVariable(variable);

      this.Context.Workflows.Add(workflow);
      await this.Context.SaveChangesAsync();

      variable.CanSwitch = false;
      var param = new TriggerParam("SwitchOn", instance)
        .AddVariableWithKey<LightSwitcherWorkflowVariable>(variable); ;

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.NotNull(triggerResult);
      Assert.False(triggerResult.HasErrors);
      Assert.Equal(instance.State, triggerResult.CurrentState);
      Assert.Equal("On", triggerResult.CurrentState);

      Assert.True(param.HasVariables);

      Assert.Single(workflow.WorkflowHistories);

      var workflowVariable = workflow.WorkflowVariables.First();
      var type = KeyBuilder.FromKey(workflowVariable.Type);
      var myDeserializedVariable = JsonSerializer.Deserialize(workflowVariable.Content, type);
      Assert.IsType<LightSwitcherWorkflowVariable>(myDeserializedVariable);

      var variableInstance = myDeserializedVariable as LightSwitcherWorkflowVariable;
      Assert.False(variableInstance.CanSwitch);
    }

    [Fact]
    public async Task WorkflowEngineService_Find_ReturnsTheDesiredIWorkflowInstance()
    {
      // Arrange
      var instance = new LightSwitcher();
      this.Context.Switchers.Add(instance);

      var workflow = Workflow.Create(instance.Id, instance.Type, instance.State, "tester");
      this.Context.Workflows.Add(workflow);
      await this.Context.SaveChangesAsync();

      // Act
      var result = this.WorkflowEngineService.Find(instance.Id, typeof(LightSwitcher));

      // Assert
      Assert.NotNull(result);
      Assert.IsType<LightSwitcher>(result);

      var resultInstance = result as LightSwitcher;
      Assert.Equal(resultInstance.Id, instance.Id);
      Assert.Equal(resultInstance.Type, instance.Type);
      Assert.Equal(resultInstance.State, instance.State);
      Assert.Equal(resultInstance.Assignee, instance.Assignee);
    }
  }
}