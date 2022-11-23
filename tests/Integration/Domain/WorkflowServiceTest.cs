using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;
using tomware.Microwf.Tests.Common.WorkflowDefinitions;
using tomware.Microwf.Tests.Integration.Utils;
using Xunit;

namespace tomware.Microwf.Tests.Integration.Domain
{
  public class WorkflowServiceTest
  {
    public TestDbContext Context { get; set; }
    public IWorkflowDefinitionProvider WorkflowDefinitionProvider { get; set; }
    public IWorkflowService WorkflowService { get; set; }

    public WorkflowServiceTest()
    {
      this.Initialize();
    }

    private void Initialize()
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

    [Fact]
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
      Assert.NotNull(result);
      Assert.Equal(2, result.Count);
    }

    [Fact]
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
      Assert.NotNull(result);
      Assert.Single(result);
    }

    [Fact]
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
      Assert.NotNull(result);
      Assert.Empty(result);
    }

    [Fact]
    public async Task WorkflowService_GetAsync_WorkflowReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetAsync(1);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(1, result.Id);
      Assert.Equal(1, result.CorrelationId);
      Assert.Equal("On", result.State);
      Assert.Equal("tester", result.Assignee);
    }

    [Fact]
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
      Assert.NotNull(result);
      Assert.Equal(1, result.Id);
      Assert.Equal(1, result.CorrelationId);
      Assert.Equal("On", result.State);
      Assert.Equal("tester", result.Assignee);
    }

    [Fact]
    public async Task WorkflowService_GetHistoryAsync_WorkflowHistoriesReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetHistoryAsync(1);

      // Assert
      Assert.NotNull(result);
      Assert.Empty(result);
    }

    [Fact]
    public async Task WorkflowService_GetVariablesAsync_WorkflowVariablesReturned()
    {
      // Arrange
      var workflows = this.GetWorkflows();
      await this.Context.Workflows.AddRangeAsync(workflows);
      await this.Context.SaveChangesAsync();

      // Act
      var result = await this.WorkflowService.GetVariablesAsync(1);

      // Assert
      Assert.NotNull(result);
      Assert.Empty(result);
    }

    [Fact]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsTwoDefinitons()
    {
      // Arrange

      // Act
      var result = this.WorkflowService.GetWorkflowDefinitions();

      // Assert
      Assert.NotNull(result);
      Assert.Equal(3, result.Count());
    }

    [Fact]
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
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Equal(HolidayApprovalWorkflow.TYPE, result.First().Type);
    }

    [Fact]
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
      Assert.Equal(expected.ToString(), diagraph);
    }

    [Fact]
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
      Assert.NotNull(result);
    }

    [Fact]
    public void WorkflowService_DotWithEmptyString_FailsWithArgumentNullException()
    {
      // Act
      Assert.Throws<ArgumentNullException>(() => this.WorkflowService.Dot(string.Empty));
    }

    [Fact]
    public void WorkflowService_DotPassingInNull_FailsWithArgumentNullException()
    {
      // Act
      Assert.Throws<ArgumentNullException>(() => this.WorkflowService.Dot(null));
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