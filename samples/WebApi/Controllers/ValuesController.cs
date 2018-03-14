using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf;

namespace WebApi.Controllers
{
  [Route("api/[controller]")]
  public class ValuesController : Controller
  {
    private readonly IServiceProvider _serviceProvider;

    public ValuesController(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;
    }

    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
      var workflowDefinitions = this._serviceProvider.GetServices<IWorkflowDefinition>();
      
      return workflowDefinitions.Select(d => d.WorkflowType);
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
