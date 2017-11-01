namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources;

  public class ServerRolesContext : List<ServerRole>, IServerRolesContext
  {
    public ServerRolesContext([NotNull] params ServerRole[] roles)
      : this((IReadOnlyCollection<ServerRole>)roles)
    {
    }

    public ServerRolesContext([NotNull] IReadOnlyCollection<ServerRole> roles)
      : base(roles)
    {
      Assert.ArgumentNotNull(roles, nameof(roles));

      Log.Info($"Initializing {GetType().FullName}");

      foreach (var role in roles)
      {
        Log.Info($"Category: {role}");
      }
    }
  }
}