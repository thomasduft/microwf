using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Domain
{
  public interface IWorkflowService
  {
    /// <summary>
    /// Returns a list of workflow instances.
    /// </summary>
    /// <param name="pagingParameters"></param>
    /// <returns></returns>
    Task<PaginatedList<WorkflowViewModel>> GetWorkflowsAsync(WorkflowSearchPagingParameters pagingParameters);

    /// <summary>
    /// Returns a workflow instance.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<WorkflowViewModel> GetAsync(int id);

    /// <summary>
    /// Returns the workflow history for a workflow instance.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<WorkflowHistoryViewModel>> GetHistoryAsync(int id);

    /// <summary>
    /// Returns the workflow variables for a workflow instance.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<WorkflowVariableViewModel>> GetVariablesAsync(int id);

    /// <summary>
    /// Returns a workflow instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="correlationId"></param>
    /// <returns></returns>
    Task<WorkflowViewModel> GetInstanceAsync(string type, int correlationId);

    /// <summary>
    /// Returns a list of workflow definitions that exist in the system.
    /// </summary>
    /// <returns></returns>
    IEnumerable<WorkflowDefinitionViewModel> GetWorkflowDefinitions();

    /// <summary>
    /// Returns the dot -> diagraph notation for the given workflow type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    string Dot(string type);

    /// <summary>
    /// Returns the dot -> diagraph notation for the given workflow type with history information.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="correlationId"></param>
    /// <returns></returns>
    Task<string> DotWithHistoryAsync(string type, int correlationId);
  }
}
