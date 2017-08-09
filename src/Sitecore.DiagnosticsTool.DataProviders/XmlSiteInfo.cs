namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;
  using System.Xml.Linq;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class XmlSiteInfo : ISite
  {
    public XmlSiteInfo(XContainer packageInfo)
    {
      HostNames = new[] { packageInfo.Element("package").Element("selectedInstance").Value };
    }

    public long Id
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public string Name
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public string ApplicationPoolName
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public IEnumerable<string> HostNames { get; }

    public State State
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public IApplicationPool ApplicationPool
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public DirectoryInfo WebRoot
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public string Protocol
    {
      get
      {
        throw new WebServerResourceNotAvailableException();
      }
    }

    public HttpWebRequest CreateWebRequest(string path)
    {
      throw new WebServerResourceNotAvailableException();
    }

    public HttpWebRequest CreateWebRequest(Uri uri)
    {
      throw new WebServerResourceNotAvailableException();
    }

    public bool IsLocal { get; } = false;
  }
}