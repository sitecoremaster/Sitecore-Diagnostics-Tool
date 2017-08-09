// <summary>
//   The base context interface for providing access to resources.
// </summary>

namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  /// <summary>
  ///   The base context interface for providing access to resources.
  /// </summary>
  public interface ITestResourceContextBase
  {
    /// <summary>
    ///   System information.
    /// </summary>
    [NotNull]
    ISystemContext System { get; }
  }
}