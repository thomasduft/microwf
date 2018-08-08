using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Common;

namespace tomware.Microwf.Engine
{
  public class UserWorkflowMappingsStore
  {
    private readonly IEnumerable<UserWorkflowMapping> _workflows;

    public IEnumerable<UserWorkflowMapping> Workflows
    {
      get
      {
        return this._workflows;
      }
    }

    public UserWorkflowMappingsStore(IEnumerable<UserWorkflowMapping> workflows)
    {
      this._workflows = workflows;
    }
  }

  public class UserWorkflowMapping
  {
    public string UserName { get; set; }
    public IList<string> WorkflowDefinitions { get; set; }
  }

  public interface IUserWorkflowMappingService
  {
    IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> workflowDefinitions);
  }

  public class NoopUserWorkflowMappingService : IUserWorkflowMappingService
  {
    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      return definitions;
    }
  }

  public class InMemoryUserWorkflowMappingService : IUserWorkflowMappingService
  {
    private readonly UserContextService _userContext;
    private readonly UserWorkflowMappingsStore _userWorkflowsStore;

    public InMemoryUserWorkflowMappingService(
      UserContextService userContext,
      UserWorkflowMappingsStore userWorkflowsStore
    )
    {
      this._userContext = userContext;
      this._userWorkflowsStore = userWorkflowsStore;
    }

    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      var userWorkflow = this._userWorkflowsStore.Workflows
        .FirstOrDefault(_ => _.UserName == this._userContext.UserName);

      return definitions.Where(_ => userWorkflow.WorkflowDefinitions.Contains(_.Type));
    }
  }
}
