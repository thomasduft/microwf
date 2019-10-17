using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using tomware.Microwf.Engine;
using WebApi.Domain;
using WebApi.Extensions;
using WebApi.Identity;

namespace WebApi
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddOptions();

      services.AddCors(o =>
      {
        o.AddPolicy("AllowAllOrigins", builder =>
        {
          builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("X-Pagination");
        });
      })
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.AddAuthorization();
      services.AddRouting(o => o.LowercaseUrls = true);

      var connection = this.Configuration["ConnectionString"];
      services
        .AddEntityFrameworkSqlite()
        .AddDbContext<DomainContext>(o => o.UseSqlite(connection));

      // Identity
      var authority = this.GetAuthority();
      // var cert = Program.GetCertificate(this.Configuration);
      // services.AddIdentityServices(authority, cert);
      services.AddIdentityServices(authority);

      // Swagger
      services.AddSwaggerDocumentation();

      // Api services
      services.AddApiServices<DomainContext>(this.Configuration);
    }

    public void Configure(
      IApplicationBuilder app,
      IHostingEnvironment env,
      ILoggerFactory loggerFactory
    )
    {
      loggerFactory.AddSerilog();

      if (env.IsDevelopment())
      {
        app.UseCors("AllowAllOrigins");
        app.UseSwaggerDocumentation();

        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler(errorApp =>
        {
          errorApp.Run(async context =>
          {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (errorFeature != null)
            {
              var logger = loggerFactory.CreateLogger("Global exception logger");
              logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
            }

            await context.Response.WriteAsync("There was an error");
          });
        });

        // app.UseHsts();
      }

      // app.UseHttpsRedirection();

      app.UseFileServer();

      app.UseAuthorization();
      app.UseIdentityServer();

      app.SubscribeMessageHandlers();

      app.UseRouting();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

    private string GetAuthority()
    {
      var domainSettings = this.Configuration.GetSection("DomainSettings");
      string schema = domainSettings.GetValue<string>("schema");
      int port = domainSettings.GetValue<int>("port");

      return $"{schema}://localhost:{port}";
    }
  }
}
