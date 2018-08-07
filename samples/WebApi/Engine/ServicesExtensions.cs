using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using tomware.Microbus.Core;
using tomware.Microwf.Core;
using WebApi.Common;

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
        services.AddTransient<WorkItemMessageHandler>();
      }

      services.AddSingleton<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine<TContext>>();
      services.AddTransient<
        IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();
      services.AddTransient<IWorkflowService, WorkflowService>();
      services
            .AddTransient<IUserWorkflowDefinitionService, NoopUserWorkflowDefinitionService>();

      return services;
    }

    public static IServiceCollection AddTestUserWorkflows(
      this IServiceCollection services,
      IEnumerable<UserWorkflows> userWorkflows
    )
    {
      services
       .AddTransient<IUserWorkflowDefinitionService, InMemoryUserWorkflowDefinitionService>();
      services.AddSingleton(new UserWorkflowsStore(userWorkflows));

      return services;
    }

    public static IApplicationBuilder SubscribeMessageHandlers(this IApplicationBuilder app)
    {
      IOptions<ProcessorConfiguration> processorConfiguration = app
        .ApplicationServices
        .GetRequiredService<IOptions<ProcessorConfiguration>>();
      if (!processorConfiguration.Value.Enabled) return app;

      var messageBus = app.ApplicationServices.GetRequiredService<IMessageBus>();

      var dispatcher = app.ApplicationServices.GetRequiredService<WorkItemMessageHandler>();
      messageBus.Subscribe<WorkItemMessageHandler, WorkItem>();

      return app;
    }
  }
}
