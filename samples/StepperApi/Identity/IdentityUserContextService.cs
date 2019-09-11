using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using tomware.Microwf.Engine;

namespace StepperApi.Identity
{
  public class IdentityUserContextService : IUserContextService
  {
    public static readonly string SYSTEM_USER = "SYSTEM";
    private readonly IHttpContextAccessor context;

    public string UserName
    {
      get
      {
        var userName = this.context.HttpContext != null
          ? this.GetUserName(this.context.HttpContext)
          : SYSTEM_USER;
        if (string.IsNullOrWhiteSpace(userName))
          throw new InvalidOperationException("UserName is null!");

        return userName;
      }
    }

    public IdentityUserContextService(IHttpContextAccessor context)
    {
      this.context = context;
    }

    private string GetUserName(HttpContext context)
    {
      if (context.User?.Identity?.Name != null)
      {
        return context.User?.Identity?.Name;
      }

      var clientIdClaim = context.User.Claims.First(c => c.Type == JwtClaimTypes.ClientId);
      if (clientIdClaim != null)
      {
        return clientIdClaim.Value;
      }

      return SYSTEM_USER;
    }
  }
}