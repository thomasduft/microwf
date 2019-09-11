using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StepperApi.Domain;
using StepperApi.Identity;
using StepperApi.Workflows.Stepper;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace StepperApi.Extensions
{
  public static class ApiServiceExtensions
  {
    public static IServiceCollection AddApiServices<TContext>(
      this IServiceCollection services,
      IConfiguration configuration
    ) where TContext : EngineDbContext
    {
      var workflowConf = CreateWorkflowConfiguration(); // GetWorkflowConfiguration(services);
      IOptions<ProcessorConfiguration> processorConf = GetProcessorConfiguration(configuration, services);

      services
       .AddWorkflowEngineServices<TContext>(workflowConf)
       .AddJobQueueServices<TContext>(processorConf.Value)
       .AddTestUserWorkflowMappings(CreateSampleUserWorkflowMappings());

      services.AddScoped<IMigrationService, MigrationService>();
      services.AddTransient<IUserContextService, IdentityUserContextService>();

      services.AddTransient<IWorkflowDefinition, StepperWorkflow>();
      services.AddTransient<IStepperService, StepperService>();


      return services;
    }

    private static WorkflowConfiguration CreateWorkflowConfiguration()
    {
      return new WorkflowConfiguration
      {
        Types = new List<WorkflowType> {
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
          UserName = "admin",
          WorkflowDefinitions = new List<string> {
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "alice",
          WorkflowDefinitions = new List<string> {
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "bob",
          WorkflowDefinitions = new List<string> {
            StepperWorkflow.TYPE
          }
        }
      };
    }

    private static IOptions<WorkflowConfiguration> GetWorkflowConfiguration(
      IConfiguration configuration,
      IServiceCollection services
    )
    {
      var workflows = configuration.GetSection("Workflows");
      services.Configure<WorkflowConfiguration>(workflows);

      return services
      .BuildServiceProvider()
      .GetRequiredService<IOptions<WorkflowConfiguration>>();
    }

    private static IOptions<ProcessorConfiguration> GetProcessorConfiguration(
      IConfiguration configuration,
      IServiceCollection services
    )
    {
      var worker = configuration.GetSection("Worker");
      services.Configure<ProcessorConfiguration>(worker);

      return services
      .BuildServiceProvider()
      .GetRequiredService<IOptions<ProcessorConfiguration>>();
    }
  }
}
