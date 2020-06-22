using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using microwf.Tests.WorkflowDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace microwf.Tests.AspNetCoreEngine
{
  [TestClass]
  public class WorkflowServiceTest
  {
    public TestDbContext Context { get; set; }
    public IWorkflowDefinitionProvider WorkflowDefinitionProvider { get; set; }
    public IWorkflowService WorkflowService { get; set; }

    [TestInitialize]
    public void Initialize()
    {
      var diHelper = new DITestHelper();
      diHelper.AddTestDbContext();
      diHelper.Services.Configure<WorkflowConfiguration>(config =>
      {
        config.Types = new List<WorkflowType> {
          new WorkflowType {
            Type = EntityOnOffWorkflow.TYPE
          }
        };
      });
      diHelper.Services.AddScoped<IWorkflowDefinitionProvider, SimpleWorkflowDefinitionProvider>();
      diHelper.Services.AddTransient<IUserWorkflowMappingService, TestUserWorkflowMappingService>(fact =>
      {
        return new TestUserWorkflowMappingService();
      });
      diHelper.Services.AddTransient<IWorkflowDefinitionDtoCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowRepository, WorkflowRepository<TestDbContext>>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService>();
      var serviceProvider = diHelper.Build();

      this.Context = serviceProvider.GetRequiredService<TestDbContext>();
      this.WorkflowDefinitionProvider = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new EntityOnOffWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new OnOffWorkflow());

      this.WorkflowService = serviceProvider.GetRequiredService<IWorkflowService>();
    }

    [TestMethod]
    public async Task WorkflowService_GetWorkflowsAsyncWithoutFilters_WorkflowsReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService
        .GetWorkflowsAsync(new WorkflowSearchPagingParameters());

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Count, 2);
    }

    [TestMethod]
    public async Task WorkflowService_GetWorkflowsAsyncWithFilters_WorkflowsReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService
        .GetWorkflowsAsync(new WorkflowSearchPagingParameters
        {
          CorrelationId = 1
        });

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Count, 1);
    }

    [TestMethod]
    public async Task WorkflowService_GetWorkflowsAsyncWithFilters_NoWorkflowsReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService
        .GetWorkflowsAsync(new WorkflowSearchPagingParameters
        {
          CorrelationId = 1,
          Assignee = "not-the-tester"
        });

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Count, 0);
    }

    [TestMethod]
    public async Task WorkflowService_GetAsync_WorkflowReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetAsync(1);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Id, 1);
      Assert.AreEqual(result.CorrelationId, 1);
      Assert.AreEqual(result.State, "On");
      Assert.AreEqual(result.Assignee, "tester");
    }

    [TestMethod]
    public async Task WorkflowService_GetInstanceAsync_WorkflowReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.Switchers.AddAsync(new LightSwitcher
      {
        Id = 1,
        Type = EntityOnOffWorkflow.TYPE,
        State = "On",
        Assignee = "tester"
      });
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetInstanceAsync(EntityOnOffWorkflow.TYPE, 1);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Id, 1);
      Assert.AreEqual(result.CorrelationId, 1);
      Assert.AreEqual(result.State, "On");
      Assert.AreEqual(result.Assignee, "tester");
    }

    [TestMethod]
    public async Task WorkflowService_GetHistoryAsync_WorkflowHistoriesReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetHistoryAsync(1);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Count(), 0);
    }

    [TestMethod]
    public async Task WorkflowService_GetVariablesAsync_WorkflowVariablesReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetVariablesAsync(1);

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(result.Count(), 0);
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsTwoDefinitons()
    {
      // Arrange

      // Act
      var result = this.WorkflowService.GetWorkflowDefinitions();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(3, result.Count());
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsOneDefiniton()
    {
      // Arrange
      var diHelper = new DITestHelper();
      diHelper.AddTestDbContext();
      diHelper.Services.AddScoped<IWorkflowDefinitionProvider, SimpleWorkflowDefinitionProvider>();
      diHelper.Services.AddTransient<IUserWorkflowMappingService, TestUserWorkflowMappingService>(fact =>
      {
        return new TestUserWorkflowMappingService(new List<IWorkflowDefinition> { new HolidayApprovalWorkflow() });
      });
      diHelper.Services.AddTransient<IWorkflowDefinitionDtoCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowRepository, WorkflowRepository<TestDbContext>>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService>();
      var serviceProvider = diHelper.Build();

      var workflowDefinitionProvider = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();
      workflowDefinitionProvider.RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      workflowDefinitionProvider.RegisterWorkflowDefinition(new OnOffWorkflow());

      var workflowService = serviceProvider.GetRequiredService<IWorkflowService>();

      // Act
      var result = workflowService.GetWorkflowDefinitions();

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
      var diagraph = this.WorkflowService.Dot(OnOffWorkflow.TYPE);

      // Assert
      Assert.AreEqual(expected.ToString(), diagraph);
    }

    [TestMethod]
    public async Task WorkflowService_DotWithHistory_ReturnsDotWithHistory()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.Switchers.AddAsync(new LightSwitcher
      {
        Id = 1,
        Type = EntityOnOffWorkflow.TYPE,
        State = "On",
        Assignee = "tester"
      });
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.DotWithHistoryAsync(EntityOnOffWorkflow.TYPE, 1);

      // Assert
      Assert.IsNotNull(result);
    }

    [TestMethod]
    public void WorkflowService_DotWithEmptyString_FailsWithArgumentNullException()
    {
      // Act
      Assert.ThrowsException<ArgumentNullException>(() => this.WorkflowService.Dot(string.Empty));
    }

    [TestMethod]
    public void WorkflowService_DotPassingInNull_FailsWithArgumentNullException()
    {
      // Act
      Assert.ThrowsException<ArgumentNullException>(() => this.WorkflowService.Dot(null));
    }

    private List<Workflow> GetWorkflows()
    {
      List<Workflow> workflows = new List<Workflow>()
      {
        new Workflow {
          Id = 1,
          State = "On",
          Type = EntityOnOffWorkflow.TYPE,
          CorrelationId = 1,
          Assignee = "tester",
          Started = SystemTime.Now()
        },
        new Workflow {
          Id = 2,
          State = "On",
          Type = EntityOnOffWorkflow.TYPE,
          CorrelationId = 2,
          Assignee = "tester",
          Started = SystemTime.Now()
        }
      };

      return workflows;
    }
  }
}