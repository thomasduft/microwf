using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Engine
{
  public class UserWorkflowMappingsStore
  {
    public IEnumerable<UserWorkflowMapping> Workflows { get; }

    public UserWorkflowMappingsStore(IEnumerable<UserWorkflowMapping> workflows)
    {
      Workflows = workflows;
    }
  }

  public class UserWorkflowMapping
  {
    public string UserName { get; set; }
    public IList<string> WorkflowDefinitions { get; set; }
  }

  public interface IUserWorkflowMappingService
  {
    /// <summary>
    /// Returns only those workflow definitions for which the current user has permissions. 
    /// </summary>
    /// <param name="workflowDefinitions"></param>
    /// <returns></returns>
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
    private readonly IUserContextService _userContext;
    private readonly UserWorkflowMappingsStore _userWorkflowsStore;

    public InMemoryUserWorkflowMappingService(
      IUserContextService userContext,
      UserWorkflowMappingsStore userWorkflowsStore
    )
    {
      _userContext = userContext;
      _userWorkflowsStore = userWorkflowsStore;
    }

    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      var userWorkflow = _userWorkflowsStore.Workflows
        .FirstOrDefault(w => w.UserName == _userContext.UserName);

      return definitions.Where(d => userWorkflow.WorkflowDefinitions.Contains(d.Type));
    }
  }
}
