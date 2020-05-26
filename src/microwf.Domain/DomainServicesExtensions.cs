using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public static class DomainServicesExtensions
  {
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
      services.AddScoped<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();

      services.AddTransient<IUserWorkflowMappingService, NoopUserWorkflowMappingService>();
      services.AddTransient<IWorkflowDefinitionDtoCreator, WorkflowDefinitionDtoCreator>();
      services.AddTransient<IWorkflowService, WorkflowService>();

      return services;
    }
  }
}
