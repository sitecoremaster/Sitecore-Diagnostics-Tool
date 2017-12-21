namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class SitecoreModuleInfo : ISitecoreModuleInfo
  {
    [NotNull]
    private readonly List<IReleaseInfo> _Releases = new List<IReleaseInfo>();

    public SitecoreModuleInfo([NotNull] string name)
    {
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<IReleaseInfo> Releases => _Releases;

    internal void AddRelease(IReleaseInfo release)
    {
      _Releases.Add(release);
    }
  }
}