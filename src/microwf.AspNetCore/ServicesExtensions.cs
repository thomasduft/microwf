using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Core;

namespace tomware.Microwf.AspNetCore
{
  public static class MicrowfServicesExtensions
  {
    public static IServiceCollection AddMicrowfServices<TContext>(
      this IServiceCollection services
    ) where TContext : DbContext
    {
      services.AddSingleton<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine<TContext>>();
      services.AddTransient<
        IWorkflowDefinitionViewModelCreator,
        ConfigurationWorkflowDefinitionViewModelCreator>();
      services.AddTransient<IWorkflowService, WorkflowService>();

      return services;
    }
  }
}
