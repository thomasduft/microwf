using tomware.Microwf.Domain;

namespace tomware.Microwf.UnitTests.Utils
{
  public class TestUserContextService : IUserContextService
  {
    public string UserName => "Tester";
  }
}