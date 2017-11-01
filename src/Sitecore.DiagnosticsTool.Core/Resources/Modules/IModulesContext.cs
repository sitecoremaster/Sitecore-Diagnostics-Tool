namespace Sitecore.DiagnosticsTool.Core.Resources.Modules
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public interface IModulesContext
  {
    /// <summary>
    ///   Information about Sitecore modules.
    /// </summary>
    [NotNull]
    IReadOnlyList<ISitecoreModuleInfo> ModulesInformation { get; }

    /// <summary>
    ///   The list of modules which installed not correctly. Actually, the modules which don't have a full set of libraries.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, IReleaseInfo> IncorrectlyInstalledModules { get; }

    /// <summary>
    ///   The list of correctly installed modules.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, IReleaseInfo> InstalledModules { get; }
  }
}