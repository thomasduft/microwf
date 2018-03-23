using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tomware.Microwf.Engine;

namespace WebApi.Workflows.Holiday
{
  [Route("api/holiday")]
  public class HolidayController : Controller
  {
    private readonly IHolidayService _service;

    public HolidayController(IHolidayService service)
    {
      this._service = service;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await this._service.GetAsync(id);

      return Ok(result);
    }

    [HttpPost("new")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> New()
    {
      var result = await this._service.NewAsync();

      return Ok(result);
    }

    [HttpPost("apply")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Apply([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.ApplyAsync(model);

      return Ok(result);
    }

    [HttpPost("approve")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Approve([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.ApproveAsync(model);

      return Ok(result);
    }

    [HttpPost("reject")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Reject([FromBody]HolidayViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.RejectAsync(model);

      return Ok(result);
    }

    [HttpGet("mywork")]
    [ProducesResponseType(typeof(IEnumerable<AssignableWorkflowViewModel>), 200)]
    public async Task<IActionResult> MyWork()
    {
      var result = await this._service.MyWorkAsync();

      return Ok(result);
    }
  }
}
