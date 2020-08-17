using tomware.Microwf.Domain;

namespace tomware.Microwf.IntegrationTests.Utils
{
  public class TestUserContextService : IUserContextService
  {
    public string UserName => "Tester";
  }
}