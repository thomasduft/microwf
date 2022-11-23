using tomware.Microwf.Core;
using tomware.Microwf.Domain;
using tomware.Microwf.Engine;
using tomware.Microwf.Infrastructure;
using WebApi.Workflows.Holiday;
using WebApi.Workflows.Issue;
using WebApi.Workflows.Stepper;

namespace WebApi.Extensions
{
  public static class ApiServiceExtensions
  {
    public static IServiceCollection AddApiServices<TContext>(
      this IServiceCollection services,
      IConfiguration configuration
    ) where TContext : EngineDbContext
    {
      var workflows = configuration.GetSection("Workflows");
      services.Configure<WorkflowConfiguration>(workflows);

      var worker = configuration.GetSection("Worker");
      services.Configure<ProcessorConfiguration>(worker);

      services
       .AddDomainServices()
       .AddInfrastructureServices<TContext>()
       .AddJobQueueServices<TContext>()
       .AddAspNetCoreEngineServices()
       .AddTestUserWorkflowMappings(CreateSampleUserWorkflowMappings());

      services.AddTransient<IUserContextService, IdentityUserContextService>();

      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
      services.AddTransient<IWorkflowDefinition, IssueTrackingWorkflow>();
      services.AddTransient<IWorkflowDefinition, StepperWorkflow>();

      services.AddTransient<IHolidayService, HolidayService>();
      services.AddTransient<IIssueService, IssueService>();
      services.AddTransient<IStepperService, StepperService>();

      return services;
    }

    private static WorkflowConfiguration CreateWorkflowConfiguration()
    {
      return new WorkflowConfiguration
      {
        Types = new List<WorkflowType> {
          new WorkflowType {
            Type = "HolidayApprovalWorkflow",
            Title = "Holiday",
            Description = "Simple holiday approval process.",
            Route = "holiday"
          },
          new WorkflowType {
            Type = "IssueTrackingWorkflow",
            Title = "Issue",
            Description = "Simple issue tracking process.",
            Route = "issue"
          },
          new WorkflowType {
            Type = "StepperWorkflow",
            Title = "Stepper",
            Description = "Dummy workflow to test workflow processor.",
            Route = ""
          }
        }
      };
    }

    private static List<UserWorkflowMapping> CreateSampleUserWorkflowMappings()
    {
      return new List<UserWorkflowMapping> {
        new UserWorkflowMapping {
          UserName = "Admin",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "Alice",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "Bob",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        }
      };
    }
  }
}