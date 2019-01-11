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
    [Authorize(Policy = Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkItem>), 200)]
    public ActionResult<IEnumerable<WorkItem>> GetSnapshot()
    {
      var result = _service.GetSnapshot();

      return Ok(result);
    }

    [HttpGet("upcommings")]
    [Authorize(Policy = Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkItem>), 200)]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetUpcommings()
    {
      var result = await _workItemService.GetUpCommingsAync();

      return Ok(result);
    }
  }
}