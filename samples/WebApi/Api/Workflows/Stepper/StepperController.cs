using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using WebApi.Common;

namespace WebApi.Workflows.Stepper
{
  [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
  [Route("api/stepper")]
  public class StepperController : Controller
  {
    private readonly IStepperService _service;

    public StepperController(IStepperService service)
    {
      this._service = service;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<StepperViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await this._service.GetAsync(id);

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), 201)]
    public async Task<IActionResult> Post([FromBody] string name)
    {
      if (string.IsNullOrEmpty(name)) return BadRequest();

      var result = await _service.CreateAsync(name);

      return Created($"api/stepper/{result}", result);
    }

    [HttpPost("process")]
    [ProducesResponseType(200)]
    public async Task Process([FromBody] ProcessStepViewModel model)
    {
      if (model == null) BadRequest();
      if (!this.ModelState.IsValid) BadRequest(this.ModelState);

      await this._service.ProcessAsync(model);

      Ok();
    }

    [HttpGet("mywork")]
    [ProducesResponseType(typeof(IEnumerable<Stepper>), 200)]
    public async Task<IActionResult> MyWork()
    {
      var result = await this._service.MyWorkAsync();

      return Ok(result);
    }
  }
}