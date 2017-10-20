namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class Site : ISite
  {
    public long Id { get; set; }

    public string Name { get; set; }

    public string ApplicationPoolName { get; set; }

    public IEnumerable<string> HostNames { get; set; }

    public State State { get; set; }

    public IApplicationPool ApplicationPool { get; set; }

    public DirectoryInfo WebRoot { get; set; }

    public string Protocol { get; set; }

    public bool IsLocal { get; set; }

    public HttpWebRequest CreateWebRequest(string path)
    {
      return CreateWebRequest(new Uri("http://" + HostNames.First() + path));
    }

    public HttpWebRequest CreateWebRequest(Uri uri)
    {
      return (HttpWebRequest)WebRequest.Create(uri);
    }
  }
}