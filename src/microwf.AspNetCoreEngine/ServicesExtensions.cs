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
      WorkflowConfiguration workflowConfiguration,
      ProcessorConfiguration processorConfiguration
    ) where TContext : EngineDbContext
    {
      if (workflowConfiguration == null)
      {
        throw new ArgumentNullException(nameof(workflowConfiguration));
      }

      if (processorConfiguration == null)
      {
        throw new ArgumentNullException(nameof(processorConfiguration));
      }

      services.Configure<WorkflowConfiguration>(config =>
      {
        config.Types = workflowConfiguration.Types;
      });

      services.Configure<ProcessorConfiguration>(config =>
      {
        config.Enabled = processorConfiguration.Enabled;
        config.Intervall = processorConfiguration.Intervall;
      });

      if (processorConfiguration.Enabled)
      {
        services.AddSingleton<IHostedService, WorkflowProcessor>();
        services.AddSingleton<IJobQueueService, JobQueueService>();
        services.AddTransient<EnqueueWorkItemMessageHandler>();
      }

      services.AddTransient<IUserContextService, UserContextService>();
      services.AddSingleton<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
      services.AddTransient<IWorkItemService, WorkItemService<TContext>>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine<TContext>>();
      services.AddTransient<IWorkflowService, WorkflowService>();
      services.AddTransient<IUserWorkflowMappingService, NoopUserWorkflowMappingService>();
      services.AddTransient<IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();

      // MessageBus
      services.AddSingleton<IMessageBus, InMemoryMessageBus>();

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
        messageBus.Subscribe<EnqueueWorkItemMessageHandler, WorkItem>();
      }

      return app;
    }
  }
}
