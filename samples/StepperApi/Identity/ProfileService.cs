using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace StepperApi.Identity
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
        context.AddRequestedClaims(user.Claims);
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