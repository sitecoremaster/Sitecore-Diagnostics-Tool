// <summary>
//   The base context interface for providing access to resources.
// </summary>

namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Resources.FileSystem;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  /// <summary>
  ///   The base context interface for providing access to resources.
  /// </summary>
  public interface ITestResourceContext : IInstanceName, ITestResourceContextBase
  {
    /// <summary>
    ///   The read-only interface to the categories list selected by user
    /// </summary>
    [NotNull]
    IServerRolesContext ServerRoles { get; }

    /// <summary>
    ///   The read-only interface for accessing File System
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IFileSystemContext FileSystem { get; }

    /// <summary>
    ///   The read-only interface for accessing Web Server (IIS)
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IWebServerContext WebServer { get; }

    /// <summary>
    ///   The read-only interface for accessing Sitecore assemblies and configuration files
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ISitecoreInformationContext SitecoreInfo { get; }

    /// <summary>
    ///   The read-only interface for accessing Sitecore and IIS Log files
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ILoggingContext Logs { get; }

    /// <summary>
    ///   The read-only interface for accessing Sitecore databases
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IDatabaseContext Databases { get; }
  }
}