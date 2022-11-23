using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApi.Extensions
{
  public static class SwaggerServiceExtensions
  {
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "MicroWF API Documentation",
          Description = "MicroWF API Documentation"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Description = "Example: \"Bearer {token}\"",
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey
        });
      });

      return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroWF API V1");
        c.DocExpansion(DocExpansion.None);
      });

      return app;
    }
  }
}