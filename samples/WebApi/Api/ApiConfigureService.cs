using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain;
using WebApi.Extensions;
using System.Text.Json.Serialization;

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
       opt.MapInboundClaims = false;
       opt.TokenValidationParameters = new TokenValidationParameters()
       {
         ValidateIssuer = false,
         ValidateAudience = false,
         NameClaimType = "name",
         RoleClaimType = "role"
       };
     });


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