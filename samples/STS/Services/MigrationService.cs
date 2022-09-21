using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace STS;

public interface IMigrationService
{
  Task EnsureMigrationAsync();
}

public class MigrationService : IMigrationService
{
  private readonly IServiceProvider _serviceProvider;

  public MigrationService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task EnsureMigrationAsync()
  {
    using var scope = _serviceProvider.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    await RegisterApplicationsAsync(scope.ServiceProvider);
    await RegisterScopesAsync(scope.ServiceProvider);

    // TODO: seed roles workflow_admin, 
    // TODO: seed users admin/alice/bob 
    await SeedRole(scope.ServiceProvider);
    await SeedUser(scope.ServiceProvider, Users.AdminUser, Roles.WorkflowAdministrator);
    await SeedUser(scope.ServiceProvider, Users.AliceUser, null);
    await SeedUser(scope.ServiceProvider, Users.BobUser, null);
  }

  private static async Task RegisterApplicationsAsync(IServiceProvider provider)
  {
    var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("spa_webclient") is null)
    {
      await manager.CreateAsync(new OpenIddictApplicationDescriptor
      {
        ClientId = "spa_webclient",
        ConsentType = ConsentTypes.Implicit,
        DisplayName = "SPA WebClient Application",
        PostLogoutRedirectUris =
          {
            new Uri("https://localhost:5001"),
            new Uri("http://localhost:4200")
          },
        RedirectUris =
          {
            new Uri("https://localhost:5001"),
            new Uri("http://localhost:4200")
          },
        Permissions =
          {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Logout,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles,
            Permissions.Prefixes.Scope + "webapi_scope"
          },
        Requirements =
          {
            Requirements.Features.ProofKeyForCodeExchange
          }
      });
    }

    if (await manager.FindByClientIdAsync("console_client") == null)
    {
      var descriptor = new OpenIddictApplicationDescriptor
      {
        ClientId = "console_client",
        DisplayName = "Console client",
        ClientSecret = "00000000-0000-0000-0000-000000000001",
        Permissions =
          {
            Permissions.GrantTypes.ClientCredentials,
            Permissions.Prefixes.Scope + "webapi_scope"
          }
      };

      await manager.CreateAsync(descriptor);
    }
  }

  private static async Task RegisterScopesAsync(IServiceProvider provider)
  {
    var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

    if (await manager.FindByNameAsync("server_scope") is null)
    {
      await manager.CreateAsync(new OpenIddictScopeDescriptor
      {
        Name = "server_scope",
        DisplayName = "Server scope access",
        Resources =
          {
            "server"
          }
      });
    }

    if (await manager.FindByNameAsync("webapi_scope") == null)
    {
      var descriptor = new OpenIddictScopeDescriptor
      {
        Name = "webapi_scope",
        DisplayName = "API Scope access for WebApi",
        Resources =
          {
            "webapi_resource"
          }
      };

      await manager.CreateAsync(descriptor);
    }
  }

  private static async Task SeedRole(IServiceProvider provider)
  {
    var manager = provider.GetRequiredService<RoleManager<IdentityRole>>();

    var role = Roles.WorkflowAdministrator;
    var roleExists = await manager.RoleExistsAsync(role);
    if (!roleExists)
    {
      var newRole = new IdentityRole(role);
      await manager.CreateAsync(newRole);
    }
  }

  private static async Task SeedUser(
    IServiceProvider provider,
    string userName,
    string? role
  )
  {
    var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();

    var user = await manager.FindByNameAsync(userName);
    if (user != null)
    {
      return;
    }

    var applicationUser = new ApplicationUser
    {
      UserName = userName,
      Email = Users.ToEmail(userName)
    };

    var userResult = await manager.CreateAsync(applicationUser, Passwords.Password);
    if (!userResult.Succeeded)
    {
      return;
    }

    await manager.SetLockoutEnabledAsync(applicationUser, false);

    if (role != null)
    {
      await manager.AddToRoleAsync(applicationUser, role);
    }
  }
}
