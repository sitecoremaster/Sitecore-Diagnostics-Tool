namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class ModulesContext : IModulesContext
  {
    [NotNull]
    private readonly IServiceClient _Client = new ServiceClient();

    [CanBeNull]
    private IReadOnlyList<ISitecoreModuleInfo> _ModulesInformation;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo> _PotentiallyInstalledModules;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo> _IncorrectlyInstalledModules;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo> _InstalledModules;

    public ModulesContext([NotNull] IReadOnlyDictionary<string, AssemblyFile> assemblies)
    {
      Assert.ArgumentNotNull(assemblies, nameof(assemblies));

      Assemblies = assemblies;
    }

    [NotNull]
    private IReadOnlyDictionary<string, AssemblyFile> Assemblies { get; }

    public virtual IReadOnlyDictionary<string, IReleaseInfo> IncorrectlyInstalledModules => _IncorrectlyInstalledModules ?? (_IncorrectlyInstalledModules = GetIncorrectModules());

    public virtual IReadOnlyDictionary<string, IReleaseInfo> InstalledModules => _InstalledModules ?? (_InstalledModules = GetCorrectModules());

    protected virtual IReadOnlyDictionary<string, IReleaseInfo> PotentiallyInstalledModules => _PotentiallyInstalledModules ?? (_PotentiallyInstalledModules = GetPotentiallyInstalledModules());

    public IReadOnlyList<ISitecoreModuleInfo> ModulesInformation => _ModulesInformation ?? (_ModulesInformation = GetModulesInformation());

    [NotNull]
    private List<ISitecoreModuleInfo> GetModulesInformation()
    {
      var rv = new List<ISitecoreModuleInfo>();
      var products = _Client.GetProductNames();
      var modules = products.Where(p => p != "Sitecore CMS");
      foreach (var moduleName in modules)
      {
        var module = new SitecoreModuleInfo(moduleName);
        var versions = _Client.GetVersions(moduleName);
        foreach (var release in versions)
        {
          {
            IReleaseInfo releaseInfo = new ReleaseInfo(release);
            var distr = release.DefaultDistribution;
            if (distr == null)
            {
              continue;
            }

            foreach (var assembly in distr.Defaults.Assemblies)
            {
              releaseInfo.AddAssembly(assembly.Value);
            }

            module.AddRelease(releaseInfo);
          }
        }

        rv.Add(module);
      }

      return rv;
    }

    [NotNull]
    private IReadOnlyDictionary<string, IReleaseInfo> GetPotentiallyInstalledModules()
    {
      var installedAssemblies = Assemblies;
      var modulesInfo = ModulesInformation;

      var installedModules = new Dictionary<string, IReleaseInfo>();

      foreach (var assembly in installedAssemblies)
      {
        foreach (var module in modulesInfo)
        {
          foreach (var release in module.Releases)
          {
            foreach (var library in release.Assemblies)
            {
              if (assembly.Value.FileName == library.FileName && assembly.Value.FileVersion == library.FileVersion
                && !installedModules.ContainsKey(module.Name)
                && !assembly.Value.FileName.StartsWith("microsoft.", StringComparison.OrdinalIgnoreCase)
                && !assembly.Value.FileName.StartsWith("system.", StringComparison.OrdinalIgnoreCase)
                && !assembly.Value.FileName.StartsWith("newtonsoft.", StringComparison.OrdinalIgnoreCase)
                && !assembly.Value.FileName.StartsWith("WebGrease.", StringComparison.OrdinalIgnoreCase)
              )
              {
                installedModules.Add(module.Name, release);
              }
            }
          }
        }
      }

      return installedModules;
    }

    [NotNull]
    private IReadOnlyDictionary<string, IReleaseInfo> GetIncorrectModules()
    {
      var incorrectModules = new Dictionary<string, IReleaseInfo>();

      foreach (var item in PotentiallyInstalledModules)
      {
        var installed = CheckIfModuleInstalled(item);
        if (!installed)
        {
          incorrectModules.Add(item.Key, item.Value);
        }
      }

      return incorrectModules;
    }

    private bool CheckIfModuleInstalled(KeyValuePair<string, IReleaseInfo> item)
    {
      foreach (var correctModule in InstalledModules)
      {
        if (correctModule.Key == item.Key)
        {
          return true;
        }
      }

      return false;
    }

    private IReadOnlyDictionary<string, IReleaseInfo> GetCorrectModules()
    {
      var correctModules = new Dictionary<string, IReleaseInfo>();

      foreach (var module in PotentiallyInstalledModules)
      {
        var correct = CheckIfModuleInstalledCorrectly(module);
        if (correct)
        {
          correctModules.Add(module.Key, module.Value);
        }
      }

      return correctModules;
    }

    private bool CheckIfModuleInstalledCorrectly(KeyValuePair<string, IReleaseInfo> module)
    {
      foreach (var assembly in module.Value.Assemblies)
      {
        var correct = CheckIfLibraryInstalled(assembly);
        if (!correct)
        {
          return false;
        }
      }

      return true;
    }

    private bool CheckIfLibraryInstalled(AssemblyFile ass)
    {
      foreach (var assembly in Assemblies)
      {
        if (ass.FileName == assembly.Value.FileName && ass.FileVersion == assembly.Value.FileVersion)
        {
          return true;
        }
      }

      return false;
    }
  }
}