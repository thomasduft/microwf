using tomware.Microwf.Domain;

namespace microwf.Tests.Utils
{
  public class TestUserContextService : IUserContextService
  {
    public string UserName => "Tester";
  }
}