using tomware.Microwf.Domain;

namespace WebApi.Extensions
{
  public class IdentityUserContextService : IUserContextService
  {
    public static readonly string SYSTEM_USER = "SYSTEM";
    private readonly IHttpContextAccessor _context;

    public string UserName
    {
      get
      {
        var userName = _context.HttpContext != null
          ? this.GetUserName(this._context.HttpContext)
          : SYSTEM_USER;
        if (string.IsNullOrWhiteSpace(userName))
          throw new InvalidOperationException("UserName is null!");

        return userName;
      }
    }

    public IdentityUserContextService(IHttpContextAccessor context)
    {
      _context = context;
    }

    private string GetUserName(HttpContext context)
    {
      if (context.User?.Identity?.Name != null)
      {
        return context.User?.Identity?.Name;
      }

      var clientIdClaim = context.User.Claims.First(c => c.Type == "client_id");
      if (clientIdClaim != null)
      {
        return clientIdClaim.Value;
      }

      return SYSTEM_USER;
    }
  }
}