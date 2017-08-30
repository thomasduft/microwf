namespace microwf.Logging
{
  public class EmptyLogger : ILog
  {
    public void Trace(string message, params object[] args)
    {
      //NOP
    }

    public void Info(string message, params object[] args)
    {
      //NOP
    }

    public void Error(string message, params object[] args)
    {
      //NOP
    }
  }
}
