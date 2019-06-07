using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApi.Domain;

namespace WebApi
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var host = BuildWebHost(args);

      // ensure seed data
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var migrator = services.GetRequiredService<IMigrationService>();
          await migrator.EnsureMigrationAsync();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }

      await host.RunAsync();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      new WebHostBuilder()
        .UseConfiguration(new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(
            "appsettings.json",
            optional: true,
            reloadOnChange: true)
          .Build())
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseKestrel()
        .UseShutdownTimeout(TimeSpan.FromSeconds(10))
        .UseStartup<Startup>()
        .UseSerilog()
        .Build();
  }
}
