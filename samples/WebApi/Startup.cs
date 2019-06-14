using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;
using WebApi.Domain;
using WebApi.Identity;
using WebApi.Workflows.Holiday;
using WebApi.Workflows.Issue;
using WebApi.Workflows.Stepper;

namespace WebApi
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Log.Logger = new LoggerConfiguration().ReadFrom
        .Configuration(configuration)
        .CreateLogger();

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
            .WithExposedHeaders("X-Pagination"); // see https://offering.solutions/blog/articles/2017/11/21/using-the-angular-material-paginator-with-aspnetcore-angular/#customercontroller
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
      var authority = this.Configuration["IdentityServer.Authority"];
      services
        .AddIdentityServer(o =>
        {
          o.IssuerUri = authority;
          o.Authentication.CookieAuthenticationScheme = "dummy";
        })
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
        .AddCookie("dummy")
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

      // Custom services
      services.AddScoped<IMigrationService, MigrationService>();

      var workflowConf = CreateWorkflowConfiguration(); // GetWorkflowConfiguration(services);
      IOptions<ProcessorConfiguration> processorConf = GetProcessorConfiguration(services);
      services
        .AddWorkflowEngineServices<DomainContext>(workflowConf)
        .AddJobQueueServices<DomainContext>(processorConf.Value)
        .AddTestUserWorkflowMappings(CreateSampleUserWorkflowMappings());

      services.AddTransient<IUserContextService, IdentityUserContextService>();

      services.AddTransient<IWorkflowDefinition, HolidayApprovalWorkflow>();
      services.AddTransient<IWorkflowDefinition, IssueTrackingWorkflow>();
      services.AddTransient<IWorkflowDefinition, StepperWorkflow>();

      services.AddTransient<IHolidayService, HolidayService>();
      services.AddTransient<IIssueService, IssueService>();
      services.AddTransient<IStepperService, StepperService>();
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
      }

      app.UseFileServer();

      app.UseIdentityServer();
      // app.UseAuthentication();

      app.SubscribeMessageHandlers();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
      });

      app.UseMvcWithDefaultRoute();
    }

    private List<UserWorkflowMapping> CreateSampleUserWorkflowMappings()
    {
      return new List<UserWorkflowMapping> {
        new UserWorkflowMapping {
          UserName = "admin",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "alice",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            IssueTrackingWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        },
        new UserWorkflowMapping {
          UserName = "bob",
          WorkflowDefinitions = new List<string> {
            HolidayApprovalWorkflow.TYPE,
            StepperWorkflow.TYPE
          }
        }
      };
    }

    private WorkflowConfiguration CreateWorkflowConfiguration()
    {
      return new WorkflowConfiguration
      {
        Types = new List<WorkflowType> {
          new WorkflowType {
            Type = "HolidayApprovalWorkflow",
            Title = "Holiday",
            Description = "Simple holiday approval process.",
            Route = "holiday"
          },
          new WorkflowType {
            Type = "IssueTrackingWorkflow",
            Title = "Issue",
            Description = "Simple issue tracking process.",
            Route = "issue"
          },
          new WorkflowType {
            Type = "StepperWorkflow",
            Title = "Stepper",
            Description = "Dummy workflow to test workflow processor.",
            Route = ""
          }
        }
      };
    }

    private IOptions<WorkflowConfiguration> GetWorkflowConfiguration(IServiceCollection services)
    {
      var workflows = this.Configuration.GetSection("Workflows");
      services.Configure<WorkflowConfiguration>(workflows);

      return services
      .BuildServiceProvider()
      .GetRequiredService<IOptions<WorkflowConfiguration>>();
    }

    private IOptions<ProcessorConfiguration> GetProcessorConfiguration(IServiceCollection services)
    {
      var worker = this.Configuration.GetSection("Worker");
      services.Configure<ProcessorConfiguration>(worker);

      return services
      .BuildServiceProvider()
      .GetRequiredService<IOptions<ProcessorConfiguration>>();
    }
  }
}
