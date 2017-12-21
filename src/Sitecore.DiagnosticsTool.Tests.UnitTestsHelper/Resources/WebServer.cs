namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class WebServer : IWebServerContext
  {
    public ISiteCollection Sites { get; set; }

    public IApplicationPoolCollection ApplicationPools { get; set; }

    public ISite CurrentSite { get; set; }

    public IServerInfo Info { get; set; }
  }
}