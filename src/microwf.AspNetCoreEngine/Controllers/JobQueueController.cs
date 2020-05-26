using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [Route("api/jobqueue")]
  public class JobQueueController : Controller
  {
    private readonly IJobQueueService service;
    private readonly IWorkItemService workItemService;

    public JobQueueController(
      IJobQueueService service,
      IWorkItemService workItemService
    )
    {
      this.service = service;
      this.workItemService = workItemService;
    }

    [HttpGet("snapshot")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<Domain.WorkItemDto>), 200)]
    public ActionResult<IEnumerable<Domain.WorkItemDto>> GetSnapshot()
    {
      var result = this.service.GetSnapshot();

      return Ok(result);
    }

    [HttpGet("upcommings")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<Domain.WorkItemDto>), 200)]
    public async Task<ActionResult<PaginatedList<Domain.WorkItemDto>>> GetUpcommings(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      PaginatedList<Domain.WorkItemDto> result
        = await this.workItemService.GetUpCommingsAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("failed")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<Domain.WorkItemDto>), 200)]
    public async Task<ActionResult<PaginatedList<Domain.WorkItemDto>>> GetFailed(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      PaginatedList<Domain.WorkItemDto> result
        = await this.workItemService.GetFailedAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpPost("reschedule")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Reschedule([FromBody] Infrastructure.WorkItemDto model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      await this.workItemService.Reschedule(model);

      return NoContent();
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<Domain.WorkItemDto> result
    )
    {
      var paginationMetadata = new
      {
        totalCount = result.AllItemsCount,
        pageSize = pagingParameters.PageSize,
        pageIndex = pagingParameters.PageIndex,
        totalPages = result.TotalPages
      };

      this.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
    }
  }
}