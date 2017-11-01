namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System.IO;
  using System.Xml.Linq;

  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class SupportPackageWebServerContext : IWebServerContext
  {
    private IServerInfo Server { get; }

    private ISite Site { get; }

    private SupportPackageWebServerContext(ISite site, IServerInfo server)
    {
      Site = site;
      Server = server;
    }

    public ISiteCollection Sites => Throw<ISiteCollection>();

    public IApplicationPoolCollection ApplicationPools => Throw<IApplicationPoolCollection>();

    public ISite CurrentSite => Site ?? Throw<ISite>();

    public IServerInfo Info => Server ?? Throw<IServerInfo>();

    public static IResource Parse(string rootPath)
    {
      var packagePath = Path.Combine(rootPath, "PackageInfo.xml");
      if (!File.Exists(packagePath))
      {
        return null;
      }

      var hardwarePath = Path.Combine(rootPath, "ServerInfo.xml");

      var server = !File.Exists(hardwarePath)
        ? null
        : File.OpenRead(hardwarePath)
          .Using(r => XDocument.Load(r))
          .With(x => new XmlServerInfo(x));

      var site = File.OpenRead(packagePath)
        .Using(r => XDocument.Load(r))
        .With(x => new XmlSiteInfo(x));

      return new SupportPackageWebServerContext(site, server);
    }

    private static T Throw<T>()
    {
      throw new WebServerResourceNotAvailableException();
    }
  }
}