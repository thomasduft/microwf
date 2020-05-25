using System;

namespace tomware.Microwf.Domain
{
  public static class SystemTime
  {
    public static Func<DateTime> Now = () => DateTimeOffset.Now.DateTime;
  }
}