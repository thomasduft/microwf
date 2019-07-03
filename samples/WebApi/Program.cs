using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebApi.Domain;

namespace WebApi
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var certificate = GetCertificate();

      var host = CreateWebHostBuilder(args, certificate).Build();

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

    public static X509Certificate2 GetCertificate()
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddEnvironmentVariables()
        .AddJsonFile(
          "appsettings.json",
          optional: false,
          reloadOnChange: true
        )
        .AddJsonFile(
          $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
          optional: true,
          reloadOnChange: true
        ).Build();

      var certificateSettings = config.GetSection("certificateSettings");
      string certificateFileName = certificateSettings.GetValue<string>("filename");
      string certificatePassword = certificateSettings.GetValue<string>("password");

      return new X509Certificate2(certificateFileName, certificatePassword);
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args, X509Certificate2 certificate) =>
      WebHost
        .CreateDefaultBuilder(args)
        .UseConfiguration(new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(
            "appsettings.json",
            optional: false,
            reloadOnChange: true
          )
          .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true,
            reloadOnChange: true
          )
          .Build())
        .ConfigureKestrel(
         options =>
         {
           options.AddServerHeader = false;
           options.Listen(IPAddress.Loopback, 5028, listenOptions =>
           {
             listenOptions.UseHttps(certificate);
           });
         })
        .UseShutdownTimeout(TimeSpan.FromSeconds(10))
        .UseStartup<Startup>()
        .UseSerilog();
  }
}
