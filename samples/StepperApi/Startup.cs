using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StepperApi.Domain;
using StepperApi.Extensions;
using StepperApi.Identity;
using tomware.Microwf.Engine;

namespace StepperApi
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
        .AddNewtonsoftJson(opt => {
          opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

      services.AddAuthentication()
        .AddIdentityServerJwt();
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

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseCors("AllowAllOrigins");
        app.UseSwaggerDocumentation();

        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseIdentityServer();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      app.SubscribeMessageHandlers();
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
