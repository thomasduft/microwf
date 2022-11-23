using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain;
using WebApi.Extensions;

namespace WebApi;

public static class ApiConfigureServices
{
  public static IServiceCollection AddApiServices(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment environment
  )
  {
    services.AddRouting(o => o.LowercaseUrls = true);

    services.AddDbContext<DomainContext>(o => o.UseSqlite(
      configuration.GetConnectionString("DefaultConnection")
    ));

    // Api services
    services.AddApiServices<DomainContext>(configuration);

    services.AddHttpContextAccessor();
    services.AddControllers()
            .AddJsonOptions(options =>
            {
              options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

    if (environment.IsDevelopment())
    {
      // Swagger
      services.AddSwaggerDocumentation();
    }

    return services;
  }
}