using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using tomware.Microbus.Core;
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

      services.AddCors(o =>
        {
          o.AddPolicy("AllowAllOrigins", builder =>
            {
              builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        })
        .AddMvc()
        .AddJsonOptions(o =>
          o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        );

      services.AddRouting(o => o.LowercaseUrls = true);

      services.AddAuthorization();

      var connection = this.Configuration["ConnectionString"];
      services
        .AddEntityFrameworkSqlite()
        .AddDbContext<DomainContext>(o => o.UseSqlite(connection));

      // Identity
      var authority = this.Configuration["IdentityServer:Authority"];
      services
        .AddIdentityServer(o => o.IssuerUri = authority)
        .AddDeveloperSigningCredential()
        .AddInMemoryPersistedGrants()
        .AddInMemoryIdentityResources(Config.GetIdentityResources())
        .AddInMemoryApiResources(Config.GetApiResources())
        .AddInMemoryClients(Config.GetClients())
        .AddTestUsers(Config.GetUsers())
        .AddJwtBearerClientAuthentication()
        .AddProfileService<ProfileService>();

      services
        .AddAuthentication(o =>
        {
          o.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
          o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
        })
        .AddIdentityServerAuthentication(o =>
        {
          o.Authority = authority;
          o.RequireHttpsMetadata = false;
          o.ApiName = "api1";
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

      // MessageBus
      services.AddSingleton<IMessageBus, InMemoryMessageBus>();
      services.AddSingleton<WorkflowProcessor>();

      // Custom services
      services.AddScoped<IEnsureDatabaseService, EnsureDatabaseService>();

      services.AddTransient<UserContextService>();

      var workflows = this.Configuration.GetSection("Workflows");
      var enableWorker = this.Configuration.GetSection("Worker").GetValue<bool>("Enable");
      services
        .AddWorkflowEngineServices<DomainContext>(workflows, enableWorker)
        .AddTestUserWorkflows(CreateUserWorkflow());

      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
      services.AddTransient<IWorkflowDefinition, IssueTrackingWorkflow>();

      services.AddTransient<IHolidayService, HolidayService>();
      services.AddTransient<IIssueService, IssueService>();
    }

    public void Configure(
      IApplicationBuilder app,
      IHostingEnvironment env,
      IApplicationLifetime appLifetime
    )
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors("AllowAllOrigins");

      app.UseIdentityServer();
      // app.UseAuthentication();

      app.UseFileServer();

      app.SubscribeMessageHandlers();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
      });

      app.UseMvcWithDefaultRoute();
    }

    private List<UserWorkflows> CreateUserWorkflow()
    {
      return new List<UserWorkflows> {
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
            HolidayApprovalWorkflow.TYPE
          }
        }
      };
    }
  }
}
