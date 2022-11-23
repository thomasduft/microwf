using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Engine
{
  public static class AspNetCoreEngineServicesExtensions
  {
    public static IServiceCollection AddAspNetCoreEngineServices(
      this IServiceCollection services
    )
    {
      services.AddTransient<IUserContextService, UserContextService>();
      services.AddTransient<IJobQueueControllerService, JobQueueControllerService>();
      services.AddTransient<IWorkflowControllerService, WorkflowControllerService>();

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

    public static IServiceCollection AddTestUserWorkflowMappings(
      this IServiceCollection services,
      IEnumerable<UserWorkflowMapping> userWorkflows
    )
    {
      if (userWorkflows == null)
      {
        throw new ArgumentNullException(nameof(userWorkflows));
      }

      services.AddSingleton(new UserWorkflowMappingsStore(userWorkflows));
      services.AddTransient<IUserWorkflowMappingService, InMemoryUserWorkflowMappingService>();

      return services;
    }
  }
}