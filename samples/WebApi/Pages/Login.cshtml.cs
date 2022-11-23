using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApi;

[AllowAnonymous]
public class LoginModel : PageModel
{
  private readonly ILogger<LoginModel> _logger;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public LoginModel(
    ILogger<LoginModel> logger,
    SignInManager<ApplicationUser> signInManager
  )
  {
    _logger = logger;
    _signInManager = signInManager;
  }

  [BindProperty]
  public InputModel Input { get; set; }

  public string ReturnUrl { get; set; }

  [TempData]
  public string ErrorMessage { get; set; }

  public class InputModel
  {
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
  }

  public async Task OnGetAsync(string returnUrl)
  {
    if (!string.IsNullOrEmpty(ErrorMessage))
    {
      ModelState.AddModelError(string.Empty, ErrorMessage);
    }

    returnUrl ??= Url.Content("~/");

    // Clear the existing external cookie to ensure a clean login process
    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    ReturnUrl = returnUrl;
  }

  public async Task<IActionResult> OnPostAsync(string returnUrl)
  {
    returnUrl ??= Url.Content("~/");

    if (ModelState.IsValid)
    {
      var result = await _signInManager.PasswordSignInAsync(
        Input?.Username,
        Input?.Password,
        Input!.RememberMe,
        lockoutOnFailure: false
      );

      if (result.Succeeded)
      {
        _logger.LogInformation("User logged in.");

        if (returnUrl == "/")
        {
          return RedirectToPage("./Logout"); // TODO: redirecto to Profile-page!
        }

        return LocalRedirect(returnUrl);
      }

      if (result.IsLockedOut)
      {
        _logger.LogWarning("User account locked out.");

        return RedirectToPage("./Lockout");
      }
      else
      {

        ModelState.AddModelError(
          string.Empty,
          $"Invalid login attempt for user '{Input.Username}'."
        );

        return Page();
      }
    }

    // If we got this far, something failed, redisplay form
    return Page();
  }
}