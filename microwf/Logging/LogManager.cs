namespace microwf.Logging
{
  internal class LogManager
  {
    private static ILog Log { get; set; }

    internal static ILog GetLogger()
    {
      return Log ?? (Log = new EmptyLogger());
    }

    internal static void SetLogger(ILog log)
    {
      Log = log ?? new EmptyLogger();
    }
  }
}
