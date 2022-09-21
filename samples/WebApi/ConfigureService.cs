using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain;
using WebApi.Extensions;

namespace WebApi;

public static class ConfigureServices
{
  public static IServiceCollection AddServer(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment environment
  )
  {
    services.AddRouting(o => o.LowercaseUrls = true);

    var authority = configuration["Authority"];

    services
     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(opt =>
     {
       opt.Authority = authority;
       opt.Audience = "webapi_scope";
       opt.RequireHttpsMetadata = false;
       opt.IncludeErrorDetails = true;
       opt.SaveToken = true;
       opt.TokenValidationParameters = new TokenValidationParameters()
       {
         ValidateIssuer = false,
         ValidateAudience = false,
         NameClaimType = "name",
         RoleClaimType = "role"
       };
     });

    var connection = configuration["ConnectionString"];
    services.AddDbContext<DomainContext>(o => o.UseSqlite(connection));

    // Api services
    services.AddApiServices<DomainContext>(configuration);

    services.AddHttpContextAccessor();
    services.AddControllers();

    if (environment.IsDevelopment())
    {
      // Swagger
      services.AddSwaggerDocumentation();
    }

    return services;
  }
}