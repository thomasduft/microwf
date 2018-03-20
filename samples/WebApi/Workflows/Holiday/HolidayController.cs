using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.AspNetCore;

namespace WebApi.Workflows.Holiday
{
  [Route("api/holiday")]
  public class HolidayController : Controller
  {
    private readonly IHolidayService _service;

    public HolidayController(IHolidayService service)
    {
      _service = service;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await _service.GetAsync(id);

      return Ok(result);
    }

    [HttpPost("new")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> New()
    {
      var result = await _service.NewAsync();

      return Ok(result);
    }

    [HttpPost("apply")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Apply([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.ApplyAsync(model);

      return Ok(result);
    }

    [HttpPost("approve")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Approve([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.ApproveAsync(model);

      return Ok(result);
    }

    [HttpPost("reject")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Reject([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.RejectAsync(model);

      return Ok(result);
    }

    [HttpGet("mywork")]
    [ProducesResponseType(typeof(IEnumerable<AssignableWorkflowViewModel>), 200)]
    public async Task<IActionResult> MyWork()
    {
      var result = await _service.MyWorkAsync();

      return Ok(result);
    }
  }
}
