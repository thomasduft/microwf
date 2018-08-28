using System;
using Microsoft.AspNetCore.Http;

namespace tomware.Microwf.Engine
{
  public interface IUserContextService
  {
    /// <summary>
    /// Returns the name of the current user.
    /// </summary>
    string UserName { get; }
  }

  public class UserContextService : IUserContextService
  {
    public static readonly string SYSTEM_USER = "SYSTEM";
    private readonly IHttpContextAccessor _context;

    public string UserName
    {
      get
      {
        var userName = _context.HttpContext != null 
          ? _context.HttpContext.User?.Identity?.Name 
          : SYSTEM_USER;
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