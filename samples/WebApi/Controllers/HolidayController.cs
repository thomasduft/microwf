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
    [ProducesResponseType(typeof(HolidayViewModel), 200)]
    public IActionResult Get()
    {
      var result = _service.GetNew();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HolidayViewModel), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await _service.GetAsync(id);

      return Ok(result);
    }
  }
}
