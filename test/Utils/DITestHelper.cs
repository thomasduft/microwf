using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class DITestHelper
  {
    public ServiceProvider Services { get; private set; }

    public DITestHelper()
    {
      Services = new ServiceCollection()
        .AddLogging()
        .BuildServiceProvider();
    }

    public ILoggerFactory GetLoggerFactory()
    {
      return this.Services.GetService<ILoggerFactory>();
    }
  }
}