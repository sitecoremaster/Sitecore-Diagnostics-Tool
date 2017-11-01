namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;

  using JetBrains.Annotations;

  public interface ISite
  {
    long Id { get; }

    [NotNull]
    string Name { get; }

    [NotNull]
    string ApplicationPoolName { get; }

    [NotNull]
    IEnumerable<string> HostNames { get; }

    State State { get; }

    [NotNull]
    IApplicationPool ApplicationPool { get; }

    [NotNull]
    DirectoryInfo WebRoot { get; }

    [NotNull]
    string Protocol { get; }

    bool IsLocal { get; }

    [NotNull]
    HttpWebRequest CreateWebRequest(string path);

    [NotNull]
    HttpWebRequest CreateWebRequest(Uri uri);
  }
}