namespace Sitecore.DiagnosticsTool.Core.Resources.Modules
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  /// <summary>
  ///   A CMS mosule and it's list of releases
  /// </summary>
  public interface ISitecoreModuleInfo
  {
    /// <summary>
    ///   Name
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    ///   List of all available releases
    /// </summary>
    [NotNull]
    IReadOnlyList<IReleaseInfo> Releases { get; }
  }
}