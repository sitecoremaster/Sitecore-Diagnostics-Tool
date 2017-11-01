namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Microsoft.Win32;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Logging;

  public static class FrameworkVersionHelper
  {
    private static IReadOnlyList<FrameworkVersion> frameworkVersions;

    //Taken from https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx
    public static IReadOnlyList<FrameworkVersion> GetFrameworkVersions()
    {
      return frameworkVersions ?? (frameworkVersions = GetFrameworkVersionsInner().ForEach(x => Log.Info($".NET Framework version detected: {x}")).Distinct().ToArray());
    }

    [NotNull]
    private static IEnumerable<FrameworkVersion> GetFrameworkVersionsInner()
    {
      var any = false;
      using (var registryKey = GetRegistryKey())
      {
        foreach (var name in registryKey.GetSubKeyNames())
        {
          var frameworkVersion = GetFrameworkVersion(name, registryKey);
          if (frameworkVersion != FrameworkVersion.Undefined)
          {
            any = true;

            yield return frameworkVersion;
          }
        }
      }

      if (!any)
      {
        yield return FrameworkVersion.Undefined;
      }
    }

    private static RegistryKey GetRegistryKey()
    {
      return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\");
    }

    public static FrameworkVersion GetFrameworkVersion(string name, RegistryKey registryKey = null)
    {
      if (name == null || !name.StartsWith("v"))
      {
        return FrameworkVersion.Undefined;
      }

      if (name.StartsWith("v2.0"))
      {
        return FrameworkVersion.v20;
      }

      if (name.StartsWith("v3.0"))
      {
        return FrameworkVersion.v30;
      }

      if (name.StartsWith("v3.5"))
      {
        return FrameworkVersion.v35;
      }

      if (!name.StartsWith("v4"))
      {
        Log.Error($"Unknown .NET version is detected: {name}");

        return FrameworkVersion.Undefined;
      }

      return GetFramework4VersionFromRegistry();
    }

    private static FrameworkVersion GetFramework4VersionFromRegistry()
    {
      using (var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
      {
        var release = registryKey?.GetValue("Release");
        if (release == null)
        {
          return FrameworkVersion.Undefined;
        }

        return ParseFrameworkVersion((int)release);
      }
    }

    private static FrameworkVersion ParseFrameworkVersion(int releaseKey)
    {
      if (releaseKey >= 393295)
      {
        return FrameworkVersion.v460;
      }

      if (releaseKey >= 379893)
      {
        return FrameworkVersion.v452;
      }

      if (releaseKey >= 378675)
      {
        return FrameworkVersion.v451;
      }

      if (releaseKey >= 378389)
      {
        return FrameworkVersion.v450;
      }

      return FrameworkVersion.v4x;
    }
  }
}