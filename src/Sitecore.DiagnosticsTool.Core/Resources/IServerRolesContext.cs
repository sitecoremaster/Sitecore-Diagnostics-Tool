namespace Sitecore.DiagnosticsTool.Core.Resources
{
  using System.Collections.Generic;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   The read-only interface to the server roles of the Sitecore instance
  /// </summary>
  public interface IServerRolesContext : IReadOnlyList<ServerRole>, IResource
  {
  }
}