using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WebApi.Domain;

namespace WebApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = BuildWebHost(args);

      // ensure seed data
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var db = services.GetRequiredService<IEnsureDatabaseService>();
          db.EnsureSeedData();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }

      host.Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      WebHost
        .CreateDefaultBuilder(args)
        .UseShutdownTimeout(TimeSpan.FromSeconds(10))
        .ConfigureAppConfiguration((context, config) =>
        {
          config
            .AddJsonFile(
              "appsettings.json",
              optional: true,
              reloadOnChange: true)
            .AddJsonFile(
              $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
              optional: true,
              reloadOnChange: true
            );
        })
        .ConfigureLogging((hostingContext, logging) =>
        {
          logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          logging.AddConsole();
          logging.AddDebug();
        })
        .UseStartup<Startup>()
        .Build();
  }
}
