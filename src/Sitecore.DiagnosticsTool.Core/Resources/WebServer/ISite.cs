namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;
  using System.IO;

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

    [NotNull]
    IApplicationPool ApplicationPool { get; }

    [NotNull]
    DirectoryInfo WebRoot { get; }
  }
}