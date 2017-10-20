namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class ReleaseInfo : IReleaseInfo
  {
    public ReleaseInfo(IRelease release)
    {
      Assemblies = new List<AssemblyFile>();
      Release = release;
    }

    public List<AssemblyFile> Assemblies { get; }

    public IRelease Release { get; }

    public void AddAssembly(AssemblyFile assembly)
    {
      Assemblies?.Add(assembly);
    }
  }
}