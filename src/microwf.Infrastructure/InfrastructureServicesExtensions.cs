using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Infrastructure
{
  public static class InfrastructureServicesExtensions
  {
    public static IServiceCollection AddInfrastructureServices<TContext>(
      this IServiceCollection services
    ) where TContext : EngineDbContext
    {
      services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

      services.AddTransient<IWorkflowRepository, WorkflowRepository<TContext>>();
      services.AddTransient<IWorkflowEngineService, WorkflowEngineService>();
      services.AddTransient<IWorkflowService, WorkflowService>();

      return services;
    }

    public static IServiceCollection AddJobQueueServices<TContext>(
      this IServiceCollection services
    ) where TContext : EngineDbContext
    {
      services.AddSingleton<IHostedService, WorkflowProcessor>();
      services.AddSingleton<IJobQueueService, JobQueueService>();

      services.AddTransient<IWorkItemRepository, WorkItemRepository<TContext>>();
      services.AddTransient<IWorkItemService, WorkItemService>();

      return services;
    }
  }
}