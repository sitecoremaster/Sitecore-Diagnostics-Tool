namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;

  public interface IApplicationPoolCollection : IEnumerable<IApplicationPool>
  {
    IApplicationPool this[string name] { get; }
  }
}