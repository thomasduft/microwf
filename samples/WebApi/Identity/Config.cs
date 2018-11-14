using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using tomware.Microwf.Engine;

namespace WebApi.Identity
{
  public class Config
  {
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
      return new List<IdentityResource>
      {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
      };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
      return new List<ApiResource>
      {
        new ApiResource("api1", "My API") {
          UserClaims = {
            JwtClaimTypes.Subject,
            JwtClaimTypes.Name,
            JwtClaimTypes.Role
          }
        }
      };
    }

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
          AllowAccessTokensViaBrowser = true,
          RequireConsent = false,
          RequireClientSecret = false,
          AllowedScopes = {
            IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
            IdentityServerConstants.StandardScopes.Profile,
            "api1"
          },
          AllowOfflineAccess = true,
          AllowedCorsOrigins = { "http://localhost:4200" }
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
          Username = "admin",
          Password = "password",
          Claims = {
            new Claim(JwtClaimTypes.Name, "admin"),
            new Claim(JwtClaimTypes.Role, Constants.WORKFLOW_ADMIN_ROLE)
          }
        },
        new TestUser
        {
          SubjectId = "2",
          Username = "alice",
          Password = "password",
          Claims = {
            new Claim(JwtClaimTypes.Name, "alice")
          }
        },
        new TestUser
        {
          SubjectId = "3",
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