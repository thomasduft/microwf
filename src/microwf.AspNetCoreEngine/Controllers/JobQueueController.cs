using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [Route("api/jobqueue")]
  public class JobQueueController : Controller
  {
    private readonly IJobQueueControllerService service;

    public JobQueueController(
      IJobQueueControllerService service
    )
    {
      this.service = service;
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
    [ProducesResponseType(typeof(PaginatedList<WorkItemViewModel>), 200)]
    public async Task<ActionResult<PaginatedList<WorkItemViewModel>>> GetUpcommings(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      var result = await this.service.GetUpCommingsAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("failed")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkItemViewModel>), 200)]
    public async Task<ActionResult<PaginatedList<WorkItemViewModel>>> GetFailed(
      [FromQuery] PagingParameters pagingParameters
    )
    {
      var result = await this.service.GetFailedAsync(pagingParameters);

      this.AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpPost("reschedule")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Reschedule([FromBody] WorkItemViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      await this.service.Reschedule(model);

      return NoContent();
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<WorkItemViewModel> result
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