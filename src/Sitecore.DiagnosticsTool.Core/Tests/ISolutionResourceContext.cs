namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  /// <summary>
  ///   The base context interface for providing access to solution-wide resources.
  /// </summary>
  public interface ISolutionResourceContext : IReadOnlyDictionary<string, IInstanceResourceContext>
  {
    /// <summary>
    ///   The defaults of the Sitecore version extracted from the sitecore\shell\sitecore.version.xml file.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ISitecoreDefaultsContext SitecoreDefaults { get; }

    /// <summary>
    ///   The version of Sitecore extracted from the sitecore\shell\sitecore.version.xml file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ISitecoreVersion SitecoreVersion { get; }

    /// <summary>
    ///   System information.
    /// </summary>
    [NotNull]
    ISystemContext System { get; }
  }
}