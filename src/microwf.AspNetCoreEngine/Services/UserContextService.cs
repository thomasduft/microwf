using System;
using Microsoft.AspNetCore.Http;
using tomware.Microwf.Domain;

namespace tomware.Microwf.Engine
{
  public class UserContextService : IUserContextService
  {
    public static readonly string SYSTEM_USER = "SYSTEM";
    private readonly IHttpContextAccessor context;

    public string UserName
    {
      get
      {
        var userName = this.context.HttpContext != null
          ? this.context.HttpContext.User?.Identity?.Name
          : SYSTEM_USER;
        if (string.IsNullOrWhiteSpace(userName))
          throw new InvalidOperationException("UserName is null!");

        return userName;
      }
    }

    public UserContextService(IHttpContextAccessor context)
    {
      this.context = context;
    }
  }
}