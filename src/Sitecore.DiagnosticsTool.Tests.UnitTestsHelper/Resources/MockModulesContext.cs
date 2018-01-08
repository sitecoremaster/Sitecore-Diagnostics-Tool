namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;

  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class MockModulesContext : IModulesContext
  {
    public MockModulesContext(Map<IReleaseInfo> installedModules)
    {
      InstalledModules = installedModules;
    }

    public IReadOnlyList<ISitecoreModuleInfo> ModulesInformation { get; } = ModulesContext.GetModulesInformation();

    public IReadOnlyDictionary<string, IReleaseInfo[]> IncorrectlyInstalledModules { get; } = new Dictionary<string, IReleaseInfo[]>();

    public IReadOnlyDictionary<string, IReleaseInfo> InstalledModules { get; }
  }
}