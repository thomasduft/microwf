using System;
using System.Linq;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace WebApi.Common
{
  // see: http://stackoverflow.com/questions/36401026/how-to-get-user-information-in-dbcontext-using-net-core

  public class UserContextService
  {
    private readonly IHttpContextAccessor _context;

    public string UserId
    {
      get
      {
        var subjectClaim = _context.HttpContext
          .User?.Claims.First(c => c.Type == JwtClaimTypes.Subject);
        if (subjectClaim == null)
          throw new InvalidOperationException("Subject claim not set!");

        return subjectClaim.Value;
      }
    }

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