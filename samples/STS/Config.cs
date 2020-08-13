using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace STS
{
  public static class Config
  {
    public static IEnumerable<IdentityResource> Ids =>
      new IdentityResource[]
      {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
      };

    public static IEnumerable<ApiScope> ApiScopes =>
       new ApiScope[]
       {
        new ApiScope
        {
          Name = "api1",
          DisplayName = "API1"
        }
       };

    public static IEnumerable<ApiResource> ApiResources =>
      new ApiResource[]
      {
        new ApiResource
        {
          Name = "api1",
          DisplayName = "API1",
          Scopes = {
           "api1"
          }
        }
      };

    public static IEnumerable<Client> Clients =>
      new Client[]
      {
        // Console client
        new Client
        {
          ClientId = "console.client",
          ClientName = "Console client",
          AllowedGrantTypes = GrantTypes.ClientCredentials,
          RequireClientSecret = false,
          ClientSecrets =
          {
            new Secret("00000000-0000-0000-0000-000000000001".Sha256())
          },
          AllowedScopes = { "api1" }
        },

        // SPA client using code flow + pkce
        new Client
        {
          ClientId = "frontend",
          ClientName = "Frontend",
          ClientUri = "http://localhost:5001",
          AllowedGrantTypes = GrantTypes.Code,
          RequirePkce = true,
          RequireClientSecret = false,
          AllowAccessTokensViaBrowser = true,
          RedirectUris =
          {
            "http://localhost:5001",
            "http://localhost:4200"
          },
          PostLogoutRedirectUris = { "http://localhost:5001/index.html", "http://localhost:4200" },
          AllowedCorsOrigins = { "http://localhost:4200", "http://localhost:5001" },
          AllowedScopes = { "openid", "profile", "api1" }
        }
      };

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
            new Claim(JwtClaimTypes.Role, "workflow_admin")
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