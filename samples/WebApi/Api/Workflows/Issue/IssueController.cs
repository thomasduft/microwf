using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Workflows.Issue
{
  [Authorize]
  [Route("api/issue")]
  public class IssueController : Controller
  {
    private readonly IIssueService _service;

    public IssueController(IIssueService service)
    {
      this._service = service;
    }

    [HttpGet("assignees")]
    [ProducesResponseType(typeof(IEnumerable<string>), 200)]
    public async Task<IActionResult> GetAssignees()
    {
      var result = await this._service.GetAssigneesAsync();

      return Ok(result);
    }

    [HttpPost("new")]
    [ProducesResponseType(typeof(IWorkflowResult<IssueViewModel>), 200)]
    public async Task<IActionResult> New()
    {
      var result = await this._service.NewAsync();

      return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IWorkflowResult<IssueViewModel>), 200)]
    public async Task<IActionResult> Get(int id)
    {
      var result = await this._service.GetAsync(id);

      return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Issue), 201)]
    public async Task<IActionResult> Post([FromBody] IssueViewModel model)
    {
      if (model == null) return BadRequest();
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _service.CreateAsync(model);

      return Created($"api/issue/{result}", result);
    }

    [HttpPost("process")]
    [ProducesResponseType(typeof(IWorkflowResult<AssigneeWorkflowResult>), 200)]
    public async Task<IActionResult> Process([FromBody] IssueViewModel model)
    {
      if (model == null) return BadRequest();
      if (!this.ModelState.IsValid) return BadRequest(this.ModelState);

      var result = await this._service.ProcessAsync(model);

      return Ok(result);
    }

    [HttpGet("mywork")]
    [ProducesResponseType(typeof(IEnumerable<Issue>), 200)]
    public async Task<IActionResult> MyWork()
    {
      var result = await this._service.MyWorkAsync();

      return Ok(result);
    }
  }
}