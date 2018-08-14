using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using tomware.Microbus.Core;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public static class MicrowfServicesExtensions
  {
    public static IServiceCollection AddWorkflowEngineServices<TContext>(
      this IServiceCollection services,
      IConfiguration workflows,
      IConfiguration processor
    ) where TContext : EngineDbContext
    {
      services.Configure<WorkflowConfiguration>(workflows);
      services.Configure<ProcessorConfiguration>(processor);

      IOptions<ProcessorConfiguration> processorConfiguration = services
        .BuildServiceProvider()
        .GetRequiredService<IOptions<ProcessorConfiguration>>();
      if (processorConfiguration.Value.Enabled)
      {
        services.AddSingleton<IHostedService, WorkflowProcessor>();
        services.AddSingleton<IJobQueueService, JobQueueService>();
        services.AddTransient<EnqueueWorkItemMessageHandler>();
      }

      services.AddTransient<UserContextService>();
      services.AddSingleton<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
      services.AddTransient<IWorkItemService, WorkItemService<TContext>>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine<TContext>>();
      services.AddTransient<IWorkflowService, WorkflowService>();
      services.AddTransient<IUserWorkflowMappingService, NoopUserWorkflowMappingService>();
      services.AddTransient<IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();

      return services;
    }

    public static IServiceCollection AddTestUserWorkflowMappings(
      this IServiceCollection services,
      IEnumerable<UserWorkflowMapping> userWorkflows
    )
    {
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

      messageBus.Subscribe<EnqueueWorkItemMessageHandler, WorkItem>();

      return app;
    }
  }
}
