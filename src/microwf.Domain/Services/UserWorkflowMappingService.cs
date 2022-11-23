using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
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

  public class NoopUserWorkflowMappingService : IUserWorkflowMappingService
  {
    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      return definitions;
    }
  }

  public class InMemoryUserWorkflowMappingService : IUserWorkflowMappingService
  {
    private readonly IUserContextService userContext;
    private readonly UserWorkflowMappingsStore userWorkflowsStore;

    public InMemoryUserWorkflowMappingService(
      IUserContextService userContext,
      UserWorkflowMappingsStore userWorkflowsStore
    )
    {
      this.userContext = userContext;
      this.userWorkflowsStore = userWorkflowsStore;
    }

    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      var userWorkflow = this.userWorkflowsStore.Workflows
        .FirstOrDefault(w => w.UserName == this.userContext.UserName);

      return definitions.Where(d => userWorkflow.WorkflowDefinitions.Contains(d.Type));
    }
  }
}