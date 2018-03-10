using tomware.Microwf;
using System.Collections.Generic;
using System.Linq;

namespace microwf.tests.Utils
{
  public class SimpleWorkflowDefinitionProvider : IWorkflowDefinitionProvider
  {
    private List<IWorkflowDefinition> _workflowDefinitions = null;

    private static SimpleWorkflowDefinitionProvider _instance;

    public static SimpleWorkflowDefinitionProvider Instance
    {
      get
      {
        if (_instance == null) _instance = new SimpleWorkflowDefinitionProvider();

        return _instance;
      }
    }

    private SimpleWorkflowDefinitionProvider()
    {
      _workflowDefinitions = new List<IWorkflowDefinition>();
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
      => _workflowDefinitions.Add(workflowDefinition);

    public IWorkflowDefinition GetWorkflowDefinition(string type)
    {
      return _workflowDefinitions.First(w => w.WorkflowType == type);
    }
  }

  //public class WebWorkflowDefinitionProvider : IWorkflowDefinitionProvider
  //{
  //  private readonly IServiceProvider _serviceProvider;

  //  public WebWorkflowDefinitionProvider(IServiceProvider serviceProvider)
  //  {
  //    _serviceProvider = serviceProvider;
  //  }

  //  public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
  //  {
  //    /**
  //    * registering done via di framework during Startup.ConfigureServices
  //    * public void ConfigureServices(IServiceCollection services)
  //    * 
  //    * Sample:
  //    *  ...
  //    *  services.AddMvc();
  //    *
  //    *  services.AddSingleton<IWorkflowDefinitionProvider, WebWorkflowDefinitionProvider>();
  //    *  services.AddTransient<ILoggerService, LoggerService>();
  //    *  services.AddTransient<IWorkflowDefinition, OnOffWorkflow>();
  //    *  services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
  //    *  ...
  //    *  
  //    */

  //    throw new NotImplementedException();
  //  }

  //  public IWorkflowDefinition GetWorkflowDefinition(string type)
  //  {
  //    return _serviceProvider
  //      .GetServices<IWorkflowDefinition>().First(t => t.WorkflowType == type);
  //  }
  //}
}
