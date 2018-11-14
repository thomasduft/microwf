using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using microwf.Tests.WorkflowDefinitions;
using tomware.Microwf.Engine;

namespace microwf.Tests.Utils
{
  public class TestUserContextService : IUserContextService
  {
    public string UserName => "Tester";
  }
}