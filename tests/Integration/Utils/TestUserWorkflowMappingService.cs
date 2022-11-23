using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Core;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Tests.Integration.Utils
{
  public class TestUserWorkflowMappingService : IUserWorkflowMappingService
  {
    private IEnumerable<IWorkflowDefinition> filters;

    public TestUserWorkflowMappingService() { }

    public TestUserWorkflowMappingService(IEnumerable<IWorkflowDefinition> filters)
    {
      this.filters = filters;
    }

    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      if (this.filters != null)
      {
        return definitions.Where(d => this.filters.Select(f => f.Type).Contains(d.Type));
      }

      return definitions;
    }
  }
}