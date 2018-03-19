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
      var type = workflowDefinition.Type;
      var startUrl = this._configuration[$"Workflows:{type}:StartUrl"];
      var description = this._configuration[$"Workflows:{type}:Description"];

      return new WorkflowDefinitionViewModel {
        Type = type,
        StartUrl = startUrl,
        Description = description
      };
    }
  }
}
