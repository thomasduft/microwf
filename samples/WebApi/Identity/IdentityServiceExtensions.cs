using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

namespace WebApi.Identity
{
  public static class IdentityServiceExtensions
  {
    public static IServiceCollection AddIdentityServices(
      this IServiceCollection services,
      string authority,
      X509Certificate2 cert
    )
    {
      services
        .AddIdentityServer(o =>
        {
          o.IssuerUri = authority;
          o.Authentication.CookieAuthenticationScheme = "dummy";
        })
        .AddSigningCredential(cert)
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

      return services;
    }
  }
}
