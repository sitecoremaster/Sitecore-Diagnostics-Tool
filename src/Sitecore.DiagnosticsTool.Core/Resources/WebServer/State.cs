namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  /// <summary>
  ///   The application pool state.
  /// </summary>
  public enum State
  {
    /// <summary>
    ///   The application pool is starting now.
    /// </summary>
    Starting,

    /// <summary>
    ///   The application pool is started.
    /// </summary>
    Started,

    /// <summary>
    ///   The application pool is stopping.
    /// </summary>
    Stopping,

    /// <summary>
    ///   The application pool is stopped.
    /// </summary>
    Stopped,

    /// <summary>
    ///   The application pool state is unknown.
    /// </summary>
    Unknown
  }
}