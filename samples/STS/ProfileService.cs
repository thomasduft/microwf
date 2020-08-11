using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace STS
{
  public class ProfileService : IProfileService
  {
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
      var sub = context.Subject?.GetSubjectId();
      if (sub == null) throw new Exception("No sub claim present");

      var user = Config.GetUsers().First(u => u.SubjectId == sub);
      if (user != null)
      {
        user.Claims.Add(new Claim(IdentityModel.JwtClaimTypes.GivenName, user.Username));

        context.IssuedClaims = user.Claims.ToList();
      }

      await Task.CompletedTask;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
      var sub = context.Subject?.GetSubjectId();
      if (sub == null) throw new Exception("No subject Id claim present");

      var user = Config.GetUsers().First(u => u.SubjectId == sub);

      context.IsActive = user != null;

      await Task.CompletedTask;
    }
  }
}