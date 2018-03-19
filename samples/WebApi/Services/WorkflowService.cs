using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Models;

namespace WebApi.Services
{
  public interface IWorkflowService
  {
    IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions();
  }

  public class WorkflowService : IWorkflowService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public WorkflowService(
      IServiceProvider serviceProvider,
      IConfiguration configuration
    )
    {
      _serviceProvider = serviceProvider;
      _configuration = configuration;
    }

    public IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions()
    {
      var workflowDefinitions = this._serviceProvider.GetServices<IWorkflowDefinition>();

      return workflowDefinitions.Select(d => this.CreateViewModel(d));
    }

    private WorkflowDefinitionViewModel CreateViewModel(IWorkflowDefinition workflowDefinition)
    {
      var workflowType = workflowDefinition.WorkflowType;
      var url = this._configuration[$"Workflows:{workflowType}:Url"];
      var description = this._configuration[$"Workflows:{workflowType}:Description"];

      return new WorkflowDefinitionViewModel {
        WorkflowType = workflowType,
        Url = url,
        Description = description
      };
    }
  }
}
