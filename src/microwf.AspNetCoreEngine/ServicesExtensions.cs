using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public static class MicrowfServicesExtensions
  {
    public static IServiceCollection AddWorkflowEngineServices<TContext>(
      this IServiceCollection services,
      WorkflowConfiguration workflowConfiguration
    ) where TContext : EngineDbContext
    {
      if (workflowConfiguration == null)
      {
        throw new ArgumentNullException(nameof(workflowConfiguration));
      }

      services.Configure<WorkflowConfiguration>(config =>
      {
        config.Types = workflowConfiguration.Types;
      });

      services.AddScoped<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
      services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

      services.AddTransient<IUserContextService, UserContextService>();
      services.AddTransient<IUserWorkflowMappingService, NoopUserWorkflowMappingService>();
      services.AddTransient<IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();

      // Data related
      services.AddTransient<IWorkflowRepository, WorkflowRepository<TContext>>();

      services.AddTransient<IWorkflowEngineService, WorkflowEngineService>();
      services.AddTransient<IWorkflowService, WorkflowService>();

      // Policies
      services.AddAuthorizationCore(options =>
      {
        options.AddPolicy(
          Constants.MANAGE_WORKFLOWS_POLICY,
          policy => policy.RequireRole(Constants.WORKFLOW_ADMIN_ROLE)
        );
      });

      return services;
    }

    public static IServiceCollection AddJobQueueServices<TContext>(
      this IServiceCollection services,
      ProcessorConfiguration processorConfiguration
    ) where TContext : EngineDbContext
    {
      if (processorConfiguration == null)
      {
        throw new ArgumentNullException(nameof(processorConfiguration));
      }

      services.Configure<ProcessorConfiguration>(config =>
      {
        config.Enabled = processorConfiguration.Enabled;
        config.Interval = processorConfiguration.Interval;
      });

      services.AddSingleton<IHostedService, WorkflowProcessor>();
      services.AddSingleton<IJobQueueService, JobQueueService>();

      services.AddTransient<IWorkItemRepository, WorkItemRepository<TContext>>();
      services.AddTransient<IWorkItemService, WorkItemService>();

      return services;
    }

    public static IServiceCollection AddTestUserWorkflowMappings(
      this IServiceCollection services,
      IEnumerable<UserWorkflowMapping> userWorkflows
    )
    {
      if (userWorkflows == null)
      {
        throw new ArgumentNullException(nameof(userWorkflows));
      }

      services.AddTransient<IUserWorkflowMappingService, InMemoryUserWorkflowMappingService>();
      services.AddSingleton(new UserWorkflowMappingsStore(userWorkflows));

      return services;
    }
  }
}
