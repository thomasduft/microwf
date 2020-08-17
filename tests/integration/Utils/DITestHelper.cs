using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using tomware.Microwf.Domain;
using tomware.Microwf.Infrastructure;

namespace tomware.Microwf.Tests.Integration.Utils
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
      this.Services.Configure<WorkflowConfiguration>(opt =>
      {
        opt.Types = workflowConfiguration.Types;
      });
      this.Services.Configure<ProcessorConfiguration>(opt =>
      {
        opt.Enabled = true;
        opt.Interval = 5000;
      });

      this.Services.AddDomainServices();
      this.Services.AddInfrastructureServices<TestDbContext>();
      this.Services.AddJobQueueServices<TestDbContext>();

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