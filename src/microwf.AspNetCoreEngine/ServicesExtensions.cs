using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using tomware.Microbus.Core;
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
      services.AddTransient<IUserContextService, UserContextService>();
      services.AddTransient<IWorkItemService, WorkItemService<TContext>>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine<TContext>>();
      services.AddTransient<IWorkflowService, WorkflowService<TContext>>();
      services.AddTransient<IWorkItemService, WorkItemService<TContext>>();
      services.AddTransient<IUserWorkflowMappingService, NoopUserWorkflowMappingService>();
      services.AddTransient<IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();

      // Setting up messaging infrastructure
      services.AddSingleton<IMessageBus, InMemoryMessageBus>();

      // Policies
      services.AddAuthorization(options =>
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

      if (processorConfiguration.Enabled)
      {
        services.AddSingleton<IHostedService, WorkflowProcessor>();
        services.AddSingleton<IJobQueueService, JobQueueService>();

        services.AddSingleton<EnqueueWorkItemMessageHandler>();
      }

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

    public static IApplicationBuilder SubscribeMessageHandlers(this IApplicationBuilder app)
    {
      IOptions<ProcessorConfiguration> processorConfiguration = app
        .ApplicationServices
        .GetRequiredService<IOptions<ProcessorConfiguration>>();
      if (!processorConfiguration.Value.Enabled) return app;

      var messageBus = app.ApplicationServices.GetRequiredService<IMessageBus>();
      if (messageBus != null)
      {
        messageBus.Subscribe<EnqueueWorkItemMessageHandler, WorkItemMessage>();
      }

      return app;
    }
  }
}
