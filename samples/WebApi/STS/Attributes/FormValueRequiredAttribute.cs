using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApi;

public sealed class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
  private readonly string _name;

  public FormValueRequiredAttribute(string name)
  {
    _name = name;
  }

  public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
  {
    var method = routeContext.HttpContext.Request.Method;
    if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase)
      || string.Equals(method, "HEAD", StringComparison.OrdinalIgnoreCase)
      || string.Equals(method, "DELETE", StringComparison.OrdinalIgnoreCase)
      || string.Equals(method, "TRACE", StringComparison.OrdinalIgnoreCase))
    {
      return false;
    }

    if (string.IsNullOrEmpty(routeContext.HttpContext.Request.ContentType))
    {
      return false;
    }

    if (!routeContext.HttpContext.Request.ContentType
      .StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
    {
      return false;
    }

    return !string.IsNullOrEmpty(routeContext.HttpContext.Request.Form[_name]);
  }
}