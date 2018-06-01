using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using WebApi.Common;

namespace tomware.Microwf.Engine
{
  public class UserWorkflowsStore 
  {
    private readonly IEnumerable<UserWorkflows> _workflows;

    public IEnumerable<UserWorkflows> Workflows {
      get {
        return this._workflows;
      }
    }

    public UserWorkflowsStore(IEnumerable<UserWorkflows> workflows)
    {
      this._workflows = workflows;
    }
  }

  public class UserWorkflows {
    public string UserName { get; set; }
    public IList<string> WorkflowDefinitions { get; set; }
  }

  public interface IUserWorkflowDefinitionService
  {
    IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> workflowDefinitions);
  }

  public class NoopUserWorkflowDefinitionService : IUserWorkflowDefinitionService
  {
    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      return definitions;
    }
  }

  public class InMemoryUserWorkflowDefinitionService : IUserWorkflowDefinitionService
  {
    private readonly UserContextService _userContext;
    private readonly UserWorkflowsStore _userWorkflowsStore;

    public InMemoryUserWorkflowDefinitionService(
      UserContextService userContext,
      UserWorkflowsStore userWorkflowsStore
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
