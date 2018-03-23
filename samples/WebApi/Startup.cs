using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Domain;
using WebApi.Workflows.Holiday;

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
      var connection = this.Configuration["ConnectionString"];

      services
        .AddEntityFrameworkSqlite()
        .AddDbContext<DomainContext>(options => options.UseSqlite(connection));

      services.AddMicrowfServices<DomainContext>();

      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
      services.AddTransient<IHolidayService, HolidayService>();

      services.AddMvc();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info
        {
          Version = "v1",
          Title = "WebAPI Documentation",
          Description = "WebAPI Documentation",
          TermsOfService = "N/A"
        });
      });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseFileServer();

      app.UseMvc();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
      });

    }
  }
}
