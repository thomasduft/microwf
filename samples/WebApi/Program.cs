using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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
      var host = CreateWebHostBuilder(args, GetConfig()).Build();

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

    public static X509Certificate2 GetCertificate(IConfiguration config)
    {
      var certificateSettings = config.GetSection("CertificateSettings");
      if (certificateSettings == null) return null;

      string certificateFileName = certificateSettings.GetValue<string>("filename");
      string certificatePassword = certificateSettings.GetValue<string>("password");
      if (!string.IsNullOrEmpty(certificateFileName) && !string.IsNullOrEmpty(certificatePassword))
      {
        return new X509Certificate2(certificateFileName, certificatePassword);
      }

      return null;
    }

    private static IWebHostBuilder CreateWebHostBuilder(
      string[] args,
      IConfigurationRoot config
    )
    {
      return WebHost
        .CreateDefaultBuilder(args)
        .UseConfiguration(config)
        .ConfigureKestrel(GetKestrelServerOptions(config))
        .UseShutdownTimeout(TimeSpan.FromSeconds(10))
        .UseStartup<Startup>()
        .UseSerilog((hostingContext, loggerConfiguration) =>
          loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        );
    }

    private static IConfigurationRoot GetConfig()
    {
      return new ConfigurationBuilder()
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
    }

    private static Action<KestrelServerOptions> GetKestrelServerOptions(IConfiguration config)
    {
      var domainSettings = config.GetSection("DomainSettings");
      var unixSocket = domainSettings.GetValue<string>("unixSocket");
      var port = domainSettings.GetValue<int>("port");
      var cert = GetCertificate(config);

      if (unixSocket != null)
      {
        return options =>
        {
          options.AddServerHeader = false;
          if (cert != null)
          {
            options.ListenUnixSocket(unixSocket, listenOptions =>
            {
              listenOptions.UseHttps(GetCertificate(config));
            });
          }
          else
          {
            options.Listen(IPAddress.Any, port, listenOptions =>
            {
              listenOptions.UseHttps();
            });
          }
        };
      }

      return options =>
      {
        options.AddServerHeader = false;
        if (cert != null)
        {
          options.Listen(IPAddress.Any, port, listenOptions =>
          {
            listenOptions.UseHttps(cert);
          });
        }
        else
        {
          options.Listen(IPAddress.Any, port, listenOptions =>
          {
            listenOptions.UseHttps();
          });
        }
      };
    }
  }
}
