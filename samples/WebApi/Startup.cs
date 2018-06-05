using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Common;
using WebApi.Domain;
using WebApi.Identity;
using WebApi.Workflows.Holiday;
using WebApi.Workflows.Issue;

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

      services.AddCors(options =>
        {
          options.AddPolicy("AllowAllOrigins", builder =>
            {
              builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        })
        .AddMvc()
        .AddJsonOptions(options =>
        {
          options.SerializerSettings.ContractResolver
            = new CamelCasePropertyNamesContractResolver();
        });

      services.AddRouting(options => options.LowercaseUrls = true);

      services.AddAuthorization();

      var connection = this.Configuration["ConnectionString"];
      services
        .AddEntityFrameworkSqlite()
        .AddDbContext<DomainContext>(options => options.UseSqlite(connection));

      // Identity
      services
        .AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryIdentityResources(Config.GetIdentityResources())
        .AddInMemoryApiResources(Config.GetApiResources())
        .AddInMemoryClients(Config.GetClients())
        .AddTestUsers(Config.GetUsers());

      services
        .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
        .AddIdentityServerAuthentication(options =>
        {
          options.Authority = this.Configuration["IdentityServer:Authority"];
          options.RequireHttpsMetadata = false;
          options.ApiName = "api1";
        });

      // Swagger
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info
        {
          Version = "v1",
          Title = "WebAPI Documentation",
          Description = "WebAPI Documentation"
        });
      });

      services.ConfigureSwaggerGen(options =>
      {
        options.AddSecurityDefinition("Bearer", new ApiKeyScheme
        {
          In = "header",
          Description = "Please insert JWT with Bearer into field",
          Name = "Authorization",
          Type = "apiKey"
        });
        options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
        {
          { "Bearer", new string[] { } }
        });
      });

      // Custom services
      services.AddScoped<IEnsureDatabaseService, EnsureDatabaseService>();

      services.AddTransient<UserContextService>();

      var workflows = this.Configuration.GetSection("Workflows");
      var enableWorker = this.Configuration.GetSection("Worker").GetValue<bool>("Enable");
      services
        .AddWorkflowEngineServices<DomainContext>(workflows, enableWorker)
        .AddTestUserWorkflows(CreateUserWorkflow());

      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
      services.AddTransient<IHolidayService, HolidayService>();
      services.AddTransient<IWorkflowDefinition, IssueTrackingWorkflow>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors("AllowAllOrigins");

      app.UseIdentityServer();
      app.UseAuthentication();

      app.UseFileServer();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
      });

      app.UseMvc();
    }

    private List<UserWorkflows> CreateUserWorkflow()
    {
      var list = new List<UserWorkflows> {
        new UserWorkflows {
          UserName = "admin",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE
          }
        },
        new UserWorkflows {
          UserName = "alice",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE
          }
        },
        new UserWorkflows {
          UserName = "bob",
          WorkflowDefinitions = new List<string> {
            // HolidayApprovalWorkflow.TYPE
          }
        }
      };

      return list;
    }
  }
}
