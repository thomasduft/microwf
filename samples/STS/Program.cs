using STS;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
  serverOptions.AddServerHeader = false;
});

// Add services to the container.
builder.Services.AddSTS(configuration, environment);

// Configure application pipeline
var app = builder.Build();
app.UseSTS(environment);

// Ensure DB migrations
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    var migrator = services.GetRequiredService<IMigrationService>();
    migrator.EnsureMigrationAsync().GetAwaiter().GetResult();
  }
  catch (Exception ex)
  {
    app.Logger.LogError(ex, "An error occurred while migrating the database.");
  }
}

app.Run();
