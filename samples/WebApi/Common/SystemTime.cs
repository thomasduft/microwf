using System;

namespace WebApi.Common
{
  public static class SystemTime
  {
    public static Func<DateTime> Now = () => DateTimeOffset.Now.DateTime;
  }
}