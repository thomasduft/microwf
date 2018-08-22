using System;
using System.Linq;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestUserWorkflowMappingService : IUserWorkflowMappingService
  {
    private IEnumerable<IWorkflowDefinition> _filters;

    public TestUserWorkflowMappingService() { }

    public TestUserWorkflowMappingService(IEnumerable<IWorkflowDefinition> filters)
    {
      _filters = filters;
    }

    public IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> definitions)
    {
      if (_filters != null) {
        return definitions.Where(_ => _filters.Select(f => f.Type).Contains(_.Type));
      }

      return definitions;
    }
  }
}