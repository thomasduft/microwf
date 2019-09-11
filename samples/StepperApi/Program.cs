using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using StepperApi.Domain;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StepperApi
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();

      // ensure database will be migrated
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
          logger.LogError(ex, "An error occurred while migrating the database.");
        }
      }

      await host.RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseUrls(GetUrls(GetConfig()))
            .UseStartup<Startup>()
            .UseSerilog((hostingContext, loggerConfiguration) =>
              loggerConfiguration
              .ReadFrom.Configuration(hostingContext.Configuration)
              .Enrich.FromLogContext()
              .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        );

    private static IConfigurationRoot GetConfig()
    {
      return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddEnvironmentVariables()
        .AddJsonFile(
          "appsettings.json",
          optional: false,
          reloadOnChange: true
        ).Build();
    }

    private static string GetUrls(IConfiguration config)
    {
      var domainSettings = config.GetSection("DomainSettings");
      var schema = domainSettings.GetValue<string>("Schema");
      var host = domainSettings.GetValue<string>("Host");
      var port = domainSettings.GetValue<int>("Port");

      return $"{schema}://{host}:{port}";
    }
  }
}
