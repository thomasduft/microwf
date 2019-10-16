using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using microwf.Tests.Utils;
using microwf.Tests.WorkflowDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

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
      diHelper.Services.AddScoped<IWorkflowDefinitionProvider, SimpleWorkflowDefinitionProvider>();
      diHelper.Services.AddTransient<IUserWorkflowMappingService, TestUserWorkflowMappingService>(fact =>
      {
        return new TestUserWorkflowMappingService();
      });
      diHelper.Services.AddTransient<IWorkflowDefinitionViewModelCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService<TestDbContext>>();
      var serviceProvider = diHelper.Build();

      this.Context = serviceProvider.GetRequiredService<TestDbContext>();
      this.WorkflowDefinitionProvider = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      this.WorkflowDefinitionProvider.RegisterWorkflowDefinition(new OnOffWorkflow());

      this.WorkflowService = serviceProvider.GetRequiredService<IWorkflowService>();
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsTwoDefinitons()
    {
      // Arrange

      // Act
      var result = this.WorkflowService.GetWorkflowDefinitions();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
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
      diHelper.Services.AddTransient<IWorkflowDefinitionViewModelCreator, TestWorkflowDefinitionViewModelCreator>();
      diHelper.Services.AddTransient<IUserContextService, TestUserContextService>();
      diHelper.Services.AddTransient<IWorkflowService, WorkflowService<TestDbContext>>();
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
  }
}