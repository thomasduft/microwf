using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace WebApi;

public static class CommonConfigureServices
{
  public static IServiceCollection AddSecurityServices(
    this IServiceCollection services
  )
  {
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

    services
      .AddAuthentication(options =>
      {
        // custom scheme defined in .AddPolicyScheme() below
        options.DefaultScheme = "JWT_OR_COOKIE";
        options.DefaultChallengeScheme = "JWT_OR_COOKIE";
      })
      // .AddCookie(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, options =>
      // {
      //   options.LoginPath = "/login";
      //   options.LogoutPath = "/logout";
      //   options.ExpireTimeSpan = TimeSpan.FromDays(1);
      // })
      .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
      {
        // options.RequireHttpsMetadata = false;
        // options.IncludeErrorDetails = true;
        // options.SaveToken = true;
        // options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
          ValidateIssuer = false,
          ValidateAudience = false,
          NameClaimType = Claims.Name,
          RoleClaimType = Claims.Role
        };
      })
      // see here: https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET
      .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
      {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
          // filter by auth type
          string authorization = context.Request.Headers[HeaderNames.Authorization];
          if (!string.IsNullOrEmpty(authorization)
            && authorization.StartsWith($"{JwtBearerDefaults.AuthenticationScheme} "))
          {
            return JwtBearerDefaults.AuthenticationScheme;
          }

          // otherwise always check for cookie auth
          return OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
        };
      });

    services
      .ConfigureApplicationCookie(options =>
      {
        options.LoginPath = new PathString("/Login");
        options.LogoutPath = new PathString("/Logout");
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
      });

    return services;
  }
}