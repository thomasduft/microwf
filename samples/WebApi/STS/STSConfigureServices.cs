using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace WebApi;

public static class STSConfigureServices
{
  public static IServiceCollection AddSTSServices(
      this IServiceCollection services,
      IConfiguration configuration,
      IWebHostEnvironment environment
    )
  {
    services.AddDbContext<STSDbContext>(options =>
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
    .AddEntityFrameworkStores<STSDbContext>()
    .AddDefaultTokenProviders();

    // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
    // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
    services.AddQuartz(options =>
    {
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
               .UseDbContext<STSDbContext>();

        options.UseQuartz();
      })
      // Register the OpenIddict server components.
      .AddServer(options =>
      {
        // Enable the authorization, device, logout, token, userinfo and verification endpoints.
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetLogoutEndpointUris("connect/logout")
               .SetTokenEndpointUris("connect/token")
               .SetIntrospectionEndpointUris("connect/introspect")
               .SetUserinfoEndpointUris("connect/userinfo");

        // Note: this sample uses the code, device, password and refresh token flows, but you
        // can enable the other flows if you need to support implicit or client credentials.
        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow()
               .AllowClientCredentialsFlow();

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