using System;

namespace tomware.Microwf.Engine
{
  public static class SystemTime
  {
    public static Func<DateTime> Now = () => DateTimeOffset.Now.DateTime;
  }
}