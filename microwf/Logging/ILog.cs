namespace microwf.Logging
{
  public interface ILog
  {
    /// <summary>
    /// Trace level logging. Use it for very fine granular information logging.
    /// </summary>
    /// <param name="message">Actual message content.</param>
    /// <param name="args">Optional arguments for message content. Message content is the format string.</param>
    void Trace(string message, params object[] args);

    /// <summary>
    /// Info level logging. Use it for reporting normal operational status.
    /// </summary>
    /// <param name="message">Actual message content.</param>
    /// <param name="args">Optional arguments for message content. Message content is the format string.</param>
    void Info(string message, params object[] args);

    /// <summary>
    /// Error level logging. Use it for reporting errors which occured during processing a request. Those errors can be recovered from.
    /// </summary>
    /// <param name="message">Actual message content.</param>
    /// <param name="args">Optional arguments for message content. Message content is the format string.</param>
    void Error(string message, params object[] args);
  }
}
