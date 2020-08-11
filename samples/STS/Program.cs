using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace STS
{
  public class Program
  {
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true
      )
      .AddEnvironmentVariables()
      .Build();

    public static int Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
         .ReadFrom.Configuration(Configuration)
         .CreateLogger();

      try
      {
        var host = CreateHostBuilder(args).Build();
        Log.Information("Starting host...");
        host.Run();
        return 0;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Host terminated unexpectedly.");
        return 1;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseConfiguration(Configuration);
          webBuilder.UseStartup<Startup>();
        });
  }
}
