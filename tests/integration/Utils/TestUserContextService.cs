using tomware.Microwf.Domain;

namespace tomware.Microwf.Tests.Integration.Utils
{
  public class TestUserContextService : IUserContextService
  {
    public string UserName => "Tester";
  }
}