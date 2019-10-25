using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class DITestHelper
  {
    public IServiceCollection Services { get; private set; }

    public DITestHelper()
    {
      this.Services = new ServiceCollection().AddLogging();
    }

    public ServiceProvider Build()
    {
      return this.Services.BuildServiceProvider();
    }

    public ServiceProvider BuildDefault(WorkflowConfiguration workflowConfiguration = null)
    {
      if (workflowConfiguration == null)
      {
        workflowConfiguration = new WorkflowConfiguration();
      }

      this.AddTestDbContext();
      this.Services.AddWorkflowEngineServices<TestDbContext>(workflowConfiguration);

      return this.Build();
    }

    public void AddTestDbContext()
    {
      this.Services.AddDbContext<TestDbContext>((o) =>
      {
        o.UseInMemoryDatabase(Guid.NewGuid().ToString())
         .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
      });
    }
  }
}