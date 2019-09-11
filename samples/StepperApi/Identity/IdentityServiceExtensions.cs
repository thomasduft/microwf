using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StepperApi.Identity
{
  public static class IdentityServiceExtensions
  {
    public static IServiceCollection AddIdentityServices(
      this IServiceCollection services,
      string authority
    )
    {
      services
        .AddIdentityServer(o =>
        {
          o.IssuerUri = authority;
          o.Authentication.CookieAuthenticationScheme = "dummy";
        })
        .AddDeveloperSigningCredential() // .AddSigningCredential(cert)
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
          o.JwtValidationClockSkew = TimeSpan.Zero;
        });

      return services;
    }
  }
}
