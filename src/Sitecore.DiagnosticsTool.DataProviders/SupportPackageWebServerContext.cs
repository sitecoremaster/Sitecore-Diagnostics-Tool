namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System.IO;
  using System.Xml.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class SupportPackageWebServerContext : IWebServerContext
  {
    public IServerInfo Server { get; }

    public ISite Site { get; }

    private SupportPackageWebServerContext([NotNull] ISite site, [NotNull] IServerInfo server)
    {
      Assert.ArgumentNotNull(site, nameof(site));
      Assert.ArgumentNotNull(server, nameof(server));
      
      Site = site;
      Server = server;
    }

    public static IResource Parse(string rootPath)
    {
      var packagePath = Path.Combine(rootPath, "PackageInfo.xml");
      if (!File.Exists(packagePath))
      {
        return null;
      }

      var hardwarePath = Path.Combine(rootPath, "ServerInfo.xml");
      if (!File.Exists(hardwarePath))
      {
        return null;
      }

      var server = File.OpenRead(hardwarePath)
          .Using(r => XDocument.Load(r))
          .With(x => new XmlServerInfo(x));

      var site = File.OpenRead(packagePath)
        .Using(r => XDocument.Load(r))
        .With(x => new XmlSiteInfo(x));

      return new SupportPackageWebServerContext(site, server);
    }
  }
}