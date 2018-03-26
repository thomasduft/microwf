using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace WebApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      WebHost
        .CreateDefaultBuilder(args)
        .UseShutdownTimeout(TimeSpan.FromSeconds(10))
        .UseStartup<Startup>()
        .Build();
  }
}
