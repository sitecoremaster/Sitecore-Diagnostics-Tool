namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using System.Collections.Generic;

  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Objects;

  /// <summary>
  ///   Release description + list of it's assemblies
  /// </summary>
  public interface IReleaseInfo
  {
    /// <summary>
    ///   Release description
    /// </summary>
    IRelease Release { get; }

    /// <summary>
    ///   List of assemblies which belong to this release
    /// </summary>
    List<AssemblyFile> Assemblies { get; }

    /// <summary>
    ///   Add assembly to the list
    /// </summary>
    /// <param name="assembly">Assembly</param>
    void AddAssembly(AssemblyFile assembly);
  }
}