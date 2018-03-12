using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tomware.Microwf;
using WebApi.Domain;
using WebApi.Services;
using WebApi.Workflows;

namespace WebApi
{
    public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var connection = this.Configuration["ConnectionString"];

      services
        .AddEntityFrameworkSqlite()
        .AddDbContext<DomainContext>(options => options.UseSqlite(connection));

      services.AddSingleton<IWorkflowDefinitionProvider, WebWorkflowDefinitionProvider>();
      services.AddTransient<IWorkflowEngine, WorkflowEngine>();
      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();
    }
  }
}
