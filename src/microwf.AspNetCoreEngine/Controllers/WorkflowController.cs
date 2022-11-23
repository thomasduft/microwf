using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [Route("api/workflow")]
  public class WorkflowController : ControllerBase
  {
    private readonly IWorkflowControllerService service;

    public WorkflowController(IWorkflowControllerService service)
    {
      this.service = service;
    }

    [HttpGet()]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkflowViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<WorkflowViewModel>>> GetWorkflows(
      [FromQuery] WorkflowSearchPagingParameters pagingParameters
    )
    {
      var result = await this.service.GetWorkflowsAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(WorkflowViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WorkflowViewModel>> Get(int id)
    {
      var result = await this.service.GetAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/history")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowHistoryViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<WorkflowHistoryViewModel>>> GetHistory(int id)
    {
      var result = await this.service.GetHistoryAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/variables")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowVariableViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<WorkflowVariableViewModel>>> GetVariables(int id)
    {
      var result = await this.service.GetVariablesAsync(id);

      return Ok(result);
    }

    [HttpGet("definitions")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionViewModel>>> GetWorkflowDefinitions()
    {
      var result = await this.service.GetWorkflowDefinitionsAsync();

      return Ok(result);
    }

    [HttpGet("dot/{type}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> Dot(string type)
    {
      var result = await this.service.DotAsync(type);

      return Ok(result);
    }

    [HttpGet("dotwithhistory/{type}/{correlationId}")]
    [ProducesResponseType(typeof(WorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkflowDto>> DotWithHistory(string type, int correlationId)
    {
      var result = await this.service.DotWithHistoryAsync(type, correlationId);

      return Ok(result);
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<WorkflowViewModel> result
    )
    {
      var paginationMetadata = new
      {
        totalCount = result.AllItemsCount,
        pageSize = pagingParameters.PageSize,
        pageIndex = pagingParameters.PageIndex,
        totalPages = result.TotalPages
      };

      this.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
    }
  }
}