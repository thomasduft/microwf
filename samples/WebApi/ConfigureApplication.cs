using Serilog;
using WebApi.Extensions;

namespace WebApi;

public static class ConfigureApplication
{
  public static void UseServer(
    this IApplicationBuilder app,
    IWebHostEnvironment environment
  )
  {
    if (environment.IsDevelopment())
    {
      app.UseCors(builder =>
      {
        builder.WithOrigins("http://localhost:4200");
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.AllowCredentials();
      });

      app.UseSwaggerDocumentation();

      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Error");
    }

    app.UseSerilogRequestLogging();

    ConsiderSpaRoutes(app);

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapRazorPages();
      endpoints.MapDefaultControllerRoute();
    });
  }

  private static void ConsiderSpaRoutes(IApplicationBuilder app)
  {
    var angularRoutes = new[]
    {
      "/home",
      "/dispatch",
      "/admin",
      "/holiday",
      "/issue",
      "/forbidden"
    };

    app.Use(async (context, next) =>
    {
      if (context.Request.Path.HasValue
        && null != angularRoutes.FirstOrDefault(
          (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
      {
        context.Request.Path = new PathString("/index.html");
        context.Response.StatusCode = StatusCodes.Status200OK;
      }

      await next();
    });
  }
}