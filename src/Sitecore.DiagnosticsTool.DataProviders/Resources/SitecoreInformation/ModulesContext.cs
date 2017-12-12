namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class ModulesContext : IModulesContext
  {
    [NotNull]
    private static readonly IServiceClient _Client = new ServiceClient();

    [CanBeNull]
    private IReadOnlyList<ISitecoreModuleInfo> _ModulesInformation;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo[]> _PotentiallyInstalledModules;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo[]> _IncorrectlyInstalledModules;

    [CanBeNull]
    private IReadOnlyDictionary<string, IReleaseInfo> _InstalledModules;

    public ModulesContext([NotNull] IReadOnlyDictionary<string, AssemblyFile> assemblies)
    {
      Assert.ArgumentNotNull(assemblies, nameof(assemblies));

      Assemblies = assemblies;
    }

    [NotNull]
    private IReadOnlyDictionary<string, AssemblyFile> Assemblies { get; }

    public virtual IReadOnlyDictionary<string, IReleaseInfo[]> IncorrectlyInstalledModules => _IncorrectlyInstalledModules ?? (_IncorrectlyInstalledModules = GetIncorrectModules());

    public virtual IReadOnlyDictionary<string, IReleaseInfo> InstalledModules => _InstalledModules ?? (_InstalledModules = GetCorrectModules());

    protected virtual IReadOnlyDictionary<string, IReleaseInfo[]> PotentiallyInstalledModules => _PotentiallyInstalledModules ?? (_PotentiallyInstalledModules = GetPotentiallyInstalledModules());

    public IReadOnlyList<ISitecoreModuleInfo> ModulesInformation => _ModulesInformation ?? (_ModulesInformation = GetModulesInformation());

    [NotNull]
    public static List<ISitecoreModuleInfo> GetModulesInformation()
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
    private IReadOnlyDictionary<string, IReleaseInfo[]> GetPotentiallyInstalledModules()
    {
      var installedAssemblies = Assemblies;
      var modulesInfo = ModulesInformation;

      var installedModules = new Dictionary<string, List<IReleaseInfo>>();

      // for each assembly in solution
      foreach (var assembly in installedAssemblies)
      {
        // for each module in SIS
        foreach (var module in modulesInfo)
        {
          var list = new List<IReleaseInfo>();

          // for each release of module in SIS
          foreach (var release in module.Releases)
          {
            foreach (var library in release.Assemblies)
            {
              // if release contains exactly the same assembly (via name and fileversion only)
              if (assembly.Value.FileName == library.FileName && assembly.Value.FileVersion == library.FileVersion)
              {
                // the release becomes potentially installed release of the given module
                list.Add(release);

                break;
              }
            }
          }

          if (list.Any())
          {
            installedModules.GetOrAdd(module.Name, new List<IReleaseInfo>()).AddRange(list);
          }
        }
      }

      return installedModules.ToDictionary(x => x.Key, x => x.Value.ToArray());
    }

    [NotNull]
    private IReadOnlyDictionary<string, IReleaseInfo[]> GetIncorrectModules()
    {
      return PotentiallyInstalledModules
        .Where(x => !InstalledModules.ContainsKey(x.Key))
        .ToDictionary(x => x.Key, x => x.Value);
    }

    private IReadOnlyDictionary<string, IReleaseInfo> GetCorrectModules()
    {
      var data = PotentiallyInstalledModules.Select(x =>
        new
        {
          x.Key,
          Release = x.Value.FirstOrDefault(CheckIfModuleInstalledCorrectly)
        });

      return data.Where(x => x.Release != null).ToDictionary(x => x.Key, x => x.Release);
    }

    private bool CheckIfModuleInstalledCorrectly(IReleaseInfo module)
    {
      foreach (var assembly in module.Assemblies)
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