using System.Collections.Generic;
using WebApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using tomware.Microwf;
using System.Linq;
using Microsoft.Extensions.Configuration;

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

      return workflowDefinitions.Select(d => new WorkflowDefinitionViewModel
      {
        WorkflowType = d.WorkflowType,
        Description = this.FindDescription(d.WorkflowType)
      });
    }

    private string FindDescription(string workflowType)
    {
      return this._configuration[$"Workflows:{workflowType}:Description"];
    }
  }
}
