using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace STS;

public static class ConfigureServices
{
  public static IServiceCollection AddSTS(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment environment
  )
  {
    services.AddDbContext<ApplicationDbContext>(options =>
    {
      // Configure the context to use Microsoft SQL Server.
      options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

      // Register the entity sets needed by OpenIddict.
      // Note: use the generic overload if you need
      // to replace the default OpenIddict entities.
      options.UseOpenIddict();
    });

    // Register the Identity services.
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
      options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Configure Identity to use the same JWT claims as OpenIddict instead
    // of the legacy WS-Federation claims it uses by default (ClaimTypes),
    // which saves you from doing the mapping in your authorization controller.
    services.Configure<IdentityOptions>(options =>
    {
      options.Password.RequireDigit = false;
      options.Password.RequiredLength = 5;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireLowercase = false;

      options.ClaimsIdentity.UserNameClaimType = Claims.Name;
      options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
      options.ClaimsIdentity.RoleClaimType = Claims.Role;
    });

    services.ConfigureApplicationCookie(options => {
      options.LoginPath = new PathString("/Login");
      options.LogoutPath = new PathString("/Logout");
    });

    // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
    // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
    services.AddQuartz(options =>
    {
      options.UseMicrosoftDependencyInjectionJobFactory();
      options.UseSimpleTypeLoader();
      options.UseInMemoryStore();
    });

    // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
    services.AddQuartzHostedService(options =>
    {
      options.WaitForJobsToComplete = true;
    });

    services.AddOpenIddict()
      // Register the OpenIddict core components.
      .AddCore(options =>
      {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();

        options.UseQuartz();
      })
      // Register the OpenIddict server components.
      .AddServer(options =>
      {
        options.SetIssuer(new Uri("https://localhost:5000/"));

        // Enable the authorization, device, logout, token, userinfo and verification endpoints.
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetLogoutEndpointUris("/connect/logout")
               .SetTokenEndpointUris("/connect/token")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetUserinfoEndpointUris("/connect/userinfo");

        // Note: this sample uses the code, device, password and refresh token flows, but you
        // can enable the other flows if you need to support implicit or client credentials.
        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        // Mark the "email", "profile", "roles" and "demo_api" scopes as supported scopes.
        options.RegisterScopes(
          Scopes.OpenId,
          Scopes.Email,
          Scopes.Profile,
          Scopes.Roles,
          "server_scope",
          "webapi_scope"
        );

        // Register the signing and encryption credentials.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // Force client applications to use Proof Key for Code Exchange (PKCE).
        options.RequireProofKeyForCodeExchange();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options.UseAspNetCore()
               .EnableStatusCodePagesIntegration()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough();

        options.DisableAccessTokenEncryption();
      })
      // Register the OpenIddict validation components.
      .AddValidation(options =>
      {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
      });

    services.AddScoped<IMigrationService, MigrationService>();

    services.AddMvc();

    return services;
  }
}
