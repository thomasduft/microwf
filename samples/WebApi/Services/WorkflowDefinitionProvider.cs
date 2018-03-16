using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using tomware.Microwf;

namespace WebApi.Services
{
  public class WorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private readonly IServiceProvider _serviceProvider;

    public WorkflowDefinitionProvider(IServiceProvider serviceProvider)
    {
      this._serviceProvider = serviceProvider;
    }

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return this._serviceProvider
        .GetServices<IWorkflowDefinition>().First(t => t.WorkflowType == type);
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
    {
       /**
        * Registering done via di framework during Startup.ConfigureServices
        * 
        * public void ConfigureServices(IServiceCollection services)
        * 
        * Sample:
        *  ...
        *  services.AddMvc();
        *
        *  services.AddSingleton<IWorkflowDefinitionProvider, WebWorkflowDefinitionProvider>();
        *  services.AddTransient<ILoggerService, LoggerService>();
        *  services.AddTransient<IWorkflowDefinition, OnOffWorkflow>();
        *  services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
        *  ...
        *  
        */

      throw new NotImplementedException();
    }
  }
}
