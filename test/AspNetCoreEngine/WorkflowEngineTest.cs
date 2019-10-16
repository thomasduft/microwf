using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using microwf.Tests.WorkflowDefinitions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkflowEngineTest
  {
    public TestDbContext Context { get; set; }
    public IWorkflowDefinitionProvider WorkflowDefinitionProvider { get; set; }

    public IWorkflowEngineService WorkflowEngineService { get; set; }

    [TestInitialize]
    public void Initialize()
    {
      var diHelper = new DITestHelper();
      diHelper.AddTestDbContext();
      diHelper.Services.AddScoped<IWorkflowDefinitionProvider, SimpleWorkflowDefinitionProvider>();
      diHelper.Services.AddTransient<IUserWorkflowMappingService, TestUserWorkflowMappingService>(fact =>
      {
        return new TestUserWorkflowMappingService();
      });
      diHelper.Services.AddTransient<IWorkflowDefinitionViewModelCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService<TestDbContext>>();
      diHelper.Services.AddTransient<IWorkflowEngineService, WorkflowEngineService<TestDbContext>>();
      var serviceProvider = diHelper.Build();

      this.Context = serviceProvider.GetRequiredService<TestDbContext>();
      this.WorkflowDefinitionProvider = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new OnOffWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new EntityOnOffWorkflow());

      this.WorkflowEngineService = serviceProvider.GetRequiredService<IWorkflowEngineService>();
    }

    [TestMethod]
    public async Task WorkflowEngine_CanTriggerAsync_CanTriggerWorkflow()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.CanTriggerAsync(param);

      // Assert
      Assert.IsNotNull(triggerResult);
      Assert.AreEqual(true, triggerResult.CanTrigger);
    }

    [TestMethod]
    public async Task WorkflowEngine_CanTriggerAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.CanTriggerAsync(null));
    }

    [TestMethod]
    public async Task WorkflowEngine_GetTriggersAsync_ReturnsPossibleTriggers()
    {
      // Arrange
      var instance = new Switcher();

      // Act
      var results = await WorkflowEngineService.GetTriggersAsync(instance);

      // Assert
      Assert.IsNotNull(results);
      Assert.AreEqual(1, results.Count());
      Assert.AreEqual("SwitchOn", results.First().TriggerName);
    }

    [TestMethod]
    public async Task WorkflowEngine_GetTriggersAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Act
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.GetTriggersAsync(null));
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsync_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.IsNotNull(triggerResult);
      Assert.IsFalse(triggerResult.HasErrors);
      Assert.AreEqual(instance.State, triggerResult.CurrentState);
      Assert.AreEqual("On", triggerResult.CurrentState);
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsyncPassInNull_ThrowsArgumentNullException()
    {
      // Act
      await Assert.ThrowsExceptionAsync<ArgumentNullException>(
        () => this.WorkflowEngineService.TriggerAsync(null));
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsyncWithEntityWorkflowInstance_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.IsNotNull(triggerResult);
      Assert.IsFalse(triggerResult.HasErrors);
      Assert.AreEqual(instance.State, triggerResult.CurrentState);
      Assert.AreEqual("On", triggerResult.CurrentState);

      Assert.AreEqual(1, this.Context.Workflows.Count());
      Assert.AreEqual(0, this.Context.Workflows.First().WorkflowVariables.Count());
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsyncWithEntityWorkflowInstanceAndNewWorkflowVariable_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new LightSwitcher();
      var workfowVariable = new LightSwitcherWorkflowVariable { CanSwitch = true };
      var param = new TriggerParam("SwitchOn", instance)
        .AddVariableWithKey<LightSwitcherWorkflowVariable>(workfowVariable);

      // Act
      var triggerResult = await this.WorkflowEngineService.TriggerAsync(param);

      // Assert
      Assert.IsNotNull(triggerResult);
      Assert.IsFalse(triggerResult.HasErrors);
      Assert.AreEqual(instance.State, triggerResult.CurrentState);
      Assert.AreEqual("On", triggerResult.CurrentState);

      Assert.AreEqual(1, this.Context.Workflows.Count());
      Assert.AreEqual(1, this.Context.Workflows.First().WorkflowVariables.Count());
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsyncWithEntityWorkflowInstanceAndExistingWorkflowVariable_ReturnsTriggerResult()
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
      Assert.IsNotNull(triggerResult);
      Assert.IsFalse(triggerResult.HasErrors);
      Assert.AreEqual(instance.State, triggerResult.CurrentState);
      Assert.AreEqual("On", triggerResult.CurrentState);

      Assert.IsTrue(param.HasVariables);

      Assert.AreEqual(1, workflow.WorkflowHistories.Count());
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsyncWithEntityWorkflowInstanceAndSameWorkflowVariable_ReturnsTriggerResult()
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
      Assert.IsNotNull(triggerResult);
      Assert.IsFalse(triggerResult.HasErrors);
      Assert.AreEqual(instance.State, triggerResult.CurrentState);
      Assert.AreEqual("On", triggerResult.CurrentState);

      Assert.IsTrue(param.HasVariables);

      Assert.AreEqual(1, workflow.WorkflowHistories.Count());

      var workflowVariable = workflow.WorkflowVariables.First();
      var type = KeyBuilder.FromKey(workflowVariable.Type);
      var myDeserializedVariable = JsonConvert.DeserializeObject(workflowVariable.Content, type);
      Assert.IsInstanceOfType(myDeserializedVariable, typeof(LightSwitcherWorkflowVariable));

      var variableInstance = myDeserializedVariable as LightSwitcherWorkflowVariable;
      Assert.IsFalse(variableInstance.CanSwitch);
    }

    [TestMethod]
    public async Task WorkflowEngine_Find_ReturnsTheDesiredIWorkflowInstance()
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
      Assert.IsNotNull(result);
      Assert.IsInstanceOfType(result, typeof(LightSwitcher));

      var resultInstance = result as LightSwitcher;
      Assert.AreEqual(resultInstance.Id, instance.Id);
      Assert.AreEqual(resultInstance.Type, instance.Type);
      Assert.AreEqual(resultInstance.State, instance.State);
      Assert.AreEqual(resultInstance.Assignee, instance.Assignee);
    }
  }
}