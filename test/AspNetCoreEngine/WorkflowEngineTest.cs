using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkflowEngineTest
  {
    public EngineDbContext Context { get; set; }
    public IWorkflowEngine WorkflowEngine { get; set; }

    [TestInitialize]
    public void Initialize()
    {
      var options = TestDbContext.CreateDbContextOptions();
      Context = new EngineDbContext(options);

      var diHelper = new DITestHelper();
      var loggerFactory = diHelper.GetLoggerFactory();
      ILogger<WorkflowEngine<EngineDbContext>> logger = loggerFactory
        .CreateLogger<WorkflowEngine<EngineDbContext>>();

      SimpleWorkflowDefinitionProvider.Instance
       .RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      this.WorkflowEngine = new WorkflowEngine<EngineDbContext>(
        Context,
        logger,
        SimpleWorkflowDefinitionProvider.Instance
      );
    }

    [TestCleanup]
    public void Cleanup()
    {
      SimpleWorkflowDefinitionProvider.Instance.Invalidate();
    }

    [TestMethod]
    public async Task WorkflowEngine_CanTriggerAsync_CanTriggerWorkflow()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await WorkflowEngine.CanTriggerAsync(param);

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
        () => this.WorkflowEngine.CanTriggerAsync(null));
    }

    [TestMethod]
    public async Task WorkflowEngine_GetTriggersAsync_ReturnsPossibleTriggers()
    {
      // Arrange
      var instance = new Switcher();

      // Act
      var results = await WorkflowEngine.GetTriggersAsync(instance);

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
        () => this.WorkflowEngine.GetTriggersAsync(null));
    }

    [TestMethod]
    public async Task WorkflowEngine_TriggerAsync_ReturnsTriggerResult()
    {
      // Arrange
      var instance = new Switcher();
      var param = new TriggerParam("SwitchOn", instance);

      // Act
      var triggerResult = await this.WorkflowEngine.TriggerAsync(param);

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
        () => this.WorkflowEngine.TriggerAsync(null));
    }

    // TODO: TriggerAsync with DB context interaction scenarios!
  }
}