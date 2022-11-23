namespace tomware.Microwf.Domain
{
  public interface IUserContextService
  {
    /// <summary>
    /// Returns the name of the current user.
    /// </summary>
    string UserName { get; }
  }
}