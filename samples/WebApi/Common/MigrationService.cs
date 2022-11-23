using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using WebApi.Domain;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace WebApi;

public interface IMigrationService
{
  Task EnsureMigrationAsync(CancellationToken cancellationToken);
}

public class MigrationService : IMigrationService
{
  private readonly IServiceProvider _serviceProvider;

  public MigrationService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task EnsureMigrationAsync(CancellationToken cancellationToken)
  {
    using var scope = _serviceProvider.CreateScope();

    var domainDbContext = scope.ServiceProvider.GetRequiredService<DomainContext>();
    await domainDbContext.Database.MigrateAsync(cancellationToken);

    var stsDbContext = scope.ServiceProvider.GetRequiredService<STSDbContext>();
    await stsDbContext.Database.MigrateAsync(cancellationToken);

    await RegisterApplicationsAsync(scope.ServiceProvider, cancellationToken);
    await RegisterScopesAsync(scope.ServiceProvider, cancellationToken);

    // TODO: seed roles workflow_admin,
    // TODO: seed users admin/alice/bob
    await SeedRole(scope.ServiceProvider);
    await SeedUser(scope.ServiceProvider, Users.AdminUser, Roles.WorkflowAdministrator);
    await SeedUser(scope.ServiceProvider, Users.AliceUser, null);
    await SeedUser(scope.ServiceProvider, Users.BobUser, null);
  }

  private static async Task RegisterApplicationsAsync(
    IServiceProvider provider,
    CancellationToken cancellationToken
  )
  {
    var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("spa_webclient", cancellationToken) is null)
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
      }, cancellationToken);
    }

    if (await manager.FindByClientIdAsync("console_client", cancellationToken) == null)
    {
      var descriptor = new OpenIddictApplicationDescriptor
      {
        ClientId = "console_client",
        DisplayName = "Console client",
        ClientSecret = "00000000-0000-0000-0000-000000000001",
        Permissions =
          {
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.ClientCredentials,
            Permissions.Prefixes.Scope + "webapi_scope"
          }
      };

      await manager.CreateAsync(descriptor, cancellationToken);
    }
  }

  private static async Task RegisterScopesAsync(
    IServiceProvider provider,
    CancellationToken cancellationToken
  )
  {
    var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

    if (await manager.FindByNameAsync("server_scope", cancellationToken) is null)
    {
      await manager.CreateAsync(new OpenIddictScopeDescriptor
      {
        Name = "server_scope",
        DisplayName = "Server scope access",
        Resources =
          {
            "server"
          }
      }, cancellationToken);
    }

    if (await manager.FindByNameAsync("webapi_scope", cancellationToken) == null)
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

      await manager.CreateAsync(descriptor, cancellationToken);
    }
  }

  private static async Task SeedRole(
    IServiceProvider provider
  )
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
    string role
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