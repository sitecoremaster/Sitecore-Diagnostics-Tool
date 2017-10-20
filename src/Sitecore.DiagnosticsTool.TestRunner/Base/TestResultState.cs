// <summary>
//   Indicates the type of the Sitecore test result.
// </summary>

namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  /// <summary>
  ///   Indicates the type of the Sitecore test result.
  /// </summary>
  public enum TestResultState
  {
    /// <summary>
    ///   Indicates that the Sitecore test failed.
    /// </summary>
    Error,

    /// <summary>
    ///   Indicates that the Sitecore test failed.
    /// </summary>
    Warning,

    /// <summary>
    ///   Indicates that the Sitecore test result is inconclusive or was not been successfully started.
    /// </summary>
    CannotRun
  }
}