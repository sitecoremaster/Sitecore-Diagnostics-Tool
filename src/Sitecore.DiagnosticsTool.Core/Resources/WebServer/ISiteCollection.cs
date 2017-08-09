namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;

  public interface ISiteCollection : IEnumerable<ISite>
  {
    ISite this[string name] { get; }
  }
}