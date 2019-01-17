using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(IEnumerable<WorkItem>), 200)]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetUpcommings()
    {
      var result = await _workItemService.GetUpCommingsAsync();

      return Ok(result);
    }

    [HttpGet("failed")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkItem>), 200)]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetFailed()
    {
      var result = await _workItemService.GetFailedAsync();

      return Ok(result);
    }

    [HttpPost("reschedule")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(400)]
    [ProducesResponseType(200)]
    public async Task<ActionResult> Reschedule([FromBody]WorkItemViewModel model)
    {
      if (model == null) BadRequest();
      if (!this.ModelState.IsValid) BadRequest(this.ModelState);

      await this._workItemService.Update(model);

      return NoContent();
    }
  }
}