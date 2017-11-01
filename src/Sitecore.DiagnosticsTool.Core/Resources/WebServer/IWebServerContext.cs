// <summary>
//   A read-only interface for accessing Web Server (IIS) sites and application pools
// </summary>

namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   A read-only interface for accessing Web Server (IIS) sites and application pools
  /// </summary>
  public interface IWebServerContext : IResource
  {
    [NotNull]
    ISiteCollection Sites { get; }

    [NotNull]
    IApplicationPoolCollection ApplicationPools { get; }

    [NotNull]
    ISite CurrentSite { get; }

    [NotNull]
    IServerInfo Info { get; }
  }
}