using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkflowServiceTest
  {

    public TestDbContext Context { get; set; }
    public IWorkflowService WorkflowService { get; set; }

    [TestInitialize]
    public void Initialize()
    {
      var options = TestDbContext.CreateDbContextOptions();
      Context = new TestDbContext(options);

      var diHelper = new DITestHelper();
      var loggerFactory = diHelper.GetLoggerFactory();
      ILogger<WorkflowService<TestDbContext>> logger = loggerFactory
        .CreateLogger<WorkflowService<TestDbContext>>();

      SimpleWorkflowDefinitionProvider.Instance
       .RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService();

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IUserContextService userContextService = new TestUserContextService();

      this.WorkflowService = new WorkflowService<TestDbContext>(
        Context,
        logger,
        SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator,
        userContextService
      );
    }

    [TestCleanup]
    public void Cleanup()
    {
      SimpleWorkflowDefinitionProvider.Instance.Invalidate();
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsTwoDefinitons()
    {
      // Act
      var result = WorkflowService.GetWorkflowDefinitions();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsOneDefiniton()
    {
      // Arrange
      SimpleWorkflowDefinitionProvider.Instance.Invalidate();

      var options = TestDbContext.CreateDbContextOptions();
      Context = new TestDbContext(options);

      var diHelper = new DITestHelper();
      var loggerFactory = diHelper.GetLoggerFactory();
      ILogger<WorkflowService<TestDbContext>> logger = loggerFactory
        .CreateLogger<WorkflowService<TestDbContext>>();

      SimpleWorkflowDefinitionProvider.Instance
       .RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      var filters = SimpleWorkflowDefinitionProvider.Instance
              .GetWorkflowDefinitions().Where(_ => _.Type == HolidayApprovalWorkflow.TYPE);
      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService(filters);

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IUserContextService userContextService = new TestUserContextService();

      var service = new WorkflowService<TestDbContext>(
        Context,
        logger,
        SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator,
        userContextService
      );

      // Act
      var result = service.GetWorkflowDefinitions();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(1, result.Count());
      Assert.AreEqual(HolidayApprovalWorkflow.TYPE, result.First().Type);
    }

    [TestMethod]
    public void WorkflowService_Dot_ReturnsADotDefinition()
    {
      // Arrange
      var expected = new StringBuilder();
      expected.AppendLine("digraph OnOffWorkflow {");
      expected.AppendLine("  On -> Off [ label = SwitchOff ];");
      expected.AppendLine("  Off -> On [ label = SwitchOn ];");
      expected.AppendLine("}");

      // Act
      var diagraph = WorkflowService.Dot(OnOffWorkflow.TYPE);

      // Assert
      Assert.AreEqual(expected.ToString(), diagraph);
    }

    [TestMethod]
    public void WorkflowService_DotWithEmptyString_FailsWithArgumentNullException()
    {
      // Act
      Assert.ThrowsException<ArgumentNullException>(() => WorkflowService.Dot(""));
    }

    [TestMethod]
    public void WorkflowService_DotPassingInNull_FailsWithArgumentNullException()
    {
      // Act
      Assert.ThrowsException<ArgumentNullException>(() => WorkflowService.Dot(null));
    }
  }
}