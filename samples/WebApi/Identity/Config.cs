using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace WebApi.Identity
{
  public class Config
  {
    // scopes define the API resources in your system
    public static IEnumerable<ApiResource> GetApiResources()
    {
      return new List<ApiResource>
      {
        new ApiResource("api1", "My API") {
          UserClaims = {
            JwtClaimTypes.Subject,
            JwtClaimTypes.Name
          }
        }
      };
    }

    // clients want to access resources (aka scopes)
    public static IEnumerable<Client> GetClients()
    {
      // client credentials client
      return new List<Client>
      {
        // resource owner password grant client
        new Client
        {
          ClientId = "ro.client",
          AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
          ClientSecrets =
          {
            new Secret("secret".Sha256())
          },
          AllowedScopes = { "api1" }
        }
      };
    }

    public static List<TestUser> GetUsers()
    {
      return new List<TestUser>
      {
        new TestUser
        {
          SubjectId = "1",
          Username = "alice",
          Password = "password",
          Claims = {
            new Claim(JwtClaimTypes.Name, "alice")
          }
        },
        new TestUser
        {
          SubjectId = "2",
          Username = "bob",
          Password = "password",
          Claims = {
            new Claim(JwtClaimTypes.Name, "bob")
          }
        }
      };
    }
  }
}