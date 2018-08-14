using System;
using Microsoft.AspNetCore.Http;

namespace tomware.Microwf.Engine
{
  // see: http://stackoverflow.com/questions/36401026/how-to-get-user-information-in-dbcontext-using-net-core

  public class UserContextService
  {
    private readonly IHttpContextAccessor _context;

    public static readonly string SYSTEM_USER = "system";

    public string UserName
    {
      get
      {
        var userName = _context.HttpContext.User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userName))
          throw new InvalidOperationException("UserName is null!");

        return userName;
      }
    }

    public UserContextService(IHttpContextAccessor context)
    {
      _context = context;
    }
  }
}