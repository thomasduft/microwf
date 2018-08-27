using System;
using System.Linq;
using System.Text;
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
    [TestCleanup]
    public void Cleanup()
    {
      SimpleWorkflowDefinitionProvider.Instance.Invalidate();
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsTwoDefinitons()
    {
      // Arrange
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new HolidayApprovalWorkflow());
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService();

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IWorkflowService service = new WorkflowService(
         SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator
      );

      // Act
      var result = service.GetWorkflowDefinitions();

      // Assert
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void WorkflowService_GetWorkflowDefinitions_ReturnsOneDefiniton()
    {
      // Arrange
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

      IWorkflowService service = new WorkflowService(
         SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator
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
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService();

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IWorkflowService service = new WorkflowService(
         SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator
      );

      var expected = new StringBuilder();
      expected.AppendLine("digraph OnOffWorkflow {");
      expected.AppendLine("  On -> Off [ label = SwitchOff ];");
      expected.AppendLine("  Off -> On [ label = SwitchOn ];");
      expected.AppendLine("}");

      // Act
      var diagraph = service.Dot(OnOffWorkflow.TYPE);

      // Assert
      Assert.AreEqual(expected.ToString(), diagraph);
    }

    [TestMethod]
    public void WorkflowService_DotWithEmptyString_FailsWithArgumentNullException()
    {
      // Arrange
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService();

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IWorkflowService service = new WorkflowService(
         SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator
      );

      // Act
      Assert.ThrowsException<ArgumentNullException>(() => service.Dot(""));
    }

    [TestMethod]
    public void WorkflowService_DotPassingInNull_FailsWithArgumentNullException()
    {
      // Arrange
      SimpleWorkflowDefinitionProvider.Instance
        .RegisterWorkflowDefinition(new OnOffWorkflow());

      IUserWorkflowMappingService userWorkflowMappingService
        = new TestUserWorkflowMappingService();

      IWorkflowDefinitionViewModelCreator workflowDefinitionViewModelCreator
        = new TestWorkflowDefinitionViewModelCreator();

      IWorkflowService service = new WorkflowService(
         SimpleWorkflowDefinitionProvider.Instance,
        userWorkflowMappingService,
        workflowDefinitionViewModelCreator
      );

      // Act
      Assert.ThrowsException<ArgumentNullException>(() => service.Dot(null));
    }
  }
}