using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        .AddJsonOptions(o =>
          o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        )
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddRouting(o => o.LowercaseUrls = true);

      services.AddAuthorization();

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

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseCors("AllowAllOrigins");
        app.UseSwaggerDocumentation();

        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();

      app.UseIdentityServer();

      app.SubscribeMessageHandlers();

      app.UseMvcWithDefaultRoute();
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
