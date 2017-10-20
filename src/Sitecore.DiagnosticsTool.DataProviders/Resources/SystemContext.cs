namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources
{
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class SystemContext : ISystemContext
  {
    public SystemContext(string code = null, string applicationInfo = null, string fileName = null)
    {
      DiagCode = code;
      PackageName = fileName;
      ApplicationInfo = applicationInfo;
    }

    public string PackageName { get; }

    public string ApplicationInfo { get; }

    public string DiagCode { get; }
  }
}