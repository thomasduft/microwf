using System.Collections.Generic;
using tomware.Microwf.Core;

namespace tomware.Microwf.Domain
{
  public interface IUserWorkflowMappingService
  {
    /// <summary>
    /// Returns only those workflow definitions for which the current user has permissions.
    /// </summary>
    /// <param name="workflowDefinitions"></param>
    /// <returns></returns>
    IEnumerable<IWorkflowDefinition> Filter(IEnumerable<IWorkflowDefinition> workflowDefinitions);
  }
}