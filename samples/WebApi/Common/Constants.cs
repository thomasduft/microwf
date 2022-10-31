namespace WebApi;

public static class Users
{
  public const string AdminUser = "Admin";
  public const string AliceUser = "Alice";
  public const string BobUser = "Bob";

  public static string ToEmail(string userName)
    => string.Format($"{userName.ToLower()}@microwf.com");
}

public static class Passwords
{
  public const string Password = "password";
}

public static class Roles
{
  public const string WorkflowAdministrator = "workflow_admin";
}