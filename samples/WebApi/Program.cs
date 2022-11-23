using Serilog;
using WebApi;
using WebApi.Domain;

var loggerConfiguration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();

Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(loggerConfiguration)
  .CreateLogger();

try
{
  Log.Information("Starting WebHost");

  var builder = WebApplication.CreateBuilder(args);
  var configuration = builder.Configuration;
  var environment = builder.Environment;

  // Configure Kestrel
  builder.WebHost.ConfigureKestrel(serverOptions =>
  {
    serverOptions.AddServerHeader = false;
  });
  builder.Host.UseSerilog();

  // Configure services
  builder.Services.AddSTSServices(configuration, environment);
  builder.Services.AddApiServices(configuration, environment);
  builder.Services.AddScoped<IMigrationService, MigrationService>();
  builder.Services.AddSecurityServices();

  // Configure application pipeline
  var app = builder.Build();
  app.UseServer(environment);

  // Ensure DB migrations
  using (var scope = app.Services.CreateScope())
  {
    var services = scope.ServiceProvider;
    try
    {
      var migrator = services.GetRequiredService<IMigrationService>();
      migrator.EnsureMigrationAsync(default).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
      app.Logger.LogError(ex, "An error occurred while migrating the database.");
    }
  }

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "WebHost terminated unexpectedly");
  return 1;
}
finally
{
  Log.CloseAndFlush();
}

return 0;