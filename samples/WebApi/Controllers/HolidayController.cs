using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
  [Route("api/holiday")]
  public class HolidayController : Controller
  {
    private readonly IHolidayService _service;

    public HolidayController(IHolidayService service)
    {
      _service = service;
    }

    [HttpGet("new")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public IActionResult New()
    {
      var result = _service.GetNew();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<HolidayViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await _service.GetAsync(id);

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
  }
}
