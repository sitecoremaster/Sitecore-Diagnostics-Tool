namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources
{
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class SystemContext : ISystemContext
  {
    public SystemContext(string applicationInfo = null)
    {
      ApplicationInfo = applicationInfo;
    }
    
    public string ApplicationInfo { get; }
  }
}