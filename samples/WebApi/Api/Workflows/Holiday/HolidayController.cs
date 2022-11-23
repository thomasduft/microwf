using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Workflows.Holiday
{
  [Authorize]
  [Route("api/holiday")]
  public class HolidayController : Controller
  {
    private readonly IHolidayService _service;

    public HolidayController(IHolidayService service)
    {
      this._service = service;
    }

    [HttpPost("new")]
    [ProducesResponseType(typeof(IWorkflowResult<ApplyHolidayViewModel>), 200)]
    public async Task<IActionResult> New()
    {
      var result = await this._service.NewAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await this._service.GetAsync(id);

      return Ok(result);
    }

    [HttpPost("apply")]
    [ProducesResponseType(typeof(IWorkflowResult<AssigneeWorkflowResult>), 200)]
    public async Task<IActionResult> Apply([FromBody] ApplyHolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.ApplyAsync(model);

      return Ok(result);
    }

    [HttpPost("approve")]
    [ProducesResponseType(typeof(IWorkflowResult<AssigneeWorkflowResult>), 200)]
    public async Task<IActionResult> Approve([FromBody] ApproveHolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.ApproveAsync(model);

      return Ok(result);
    }

    [HttpPost("reject")]
    [ProducesResponseType(typeof(IWorkflowResult<AssigneeWorkflowResult>), 200)]
    public async Task<IActionResult> Reject([FromBody] ApproveHolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.RejectAsync(model);

      return Ok(result);
    }

    [HttpGet("mywork")]
    [ProducesResponseType(typeof(IEnumerable<Holiday>), 200)]
    public async Task<IActionResult> MyWork()
    {
      var result = await this._service.MyWorkAsync();

      return Ok(result);
    }
  }
}