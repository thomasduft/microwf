using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
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
        .GetServices<IWorkflowDefinition>().First(t => t.Type == type);
    }

    public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
    {
      throw new NotImplementedException();
    }
  }
}
