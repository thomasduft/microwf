using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [Route("api/jobqueue")]
  public class JobQueueController : Controller
  {
    private readonly IJobQueueService _service;
    private readonly IWorkItemService _workItemService;

    public JobQueueController(
      IJobQueueService service,
      IWorkItemService workItemService
    )
    {
      _service = service;
      _workItemService = workItemService;
    }

    [HttpGet("snapshot")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkItem>), 200)]
    public ActionResult<IEnumerable<WorkItem>> GetSnapshot()
    {
      var result = _service.GetSnapshot();

      return Ok(result);
    }

    [HttpGet("upcommings")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkItem>), 200)]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetUpcommings(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      PaginatedList<WorkItem> result
        = await _workItemService.GetUpCommingsAsync(pagingParameters);

      AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("failed")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkItem>), 200)]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetFailed(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      PaginatedList<WorkItem> result
        = await _workItemService.GetFailedAsync(pagingParameters);

      AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpPost("reschedule")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Reschedule([FromBody]WorkItemViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      await this._workItemService.Reschedule(model);

      return NoContent();
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<WorkItem> result
    )
    {
      var paginationMetadata = new
      {
        totalCount = result.AllItemsCount,
        pageSize = pagingParameters.PageSize,
        pageIndex = pagingParameters.PageIndex,
        totalPages = result.TotalPages
      };

      Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
    }
  }
}