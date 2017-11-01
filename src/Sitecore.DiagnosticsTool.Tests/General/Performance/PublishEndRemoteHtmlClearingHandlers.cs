namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class PublishEndRemoteHtmlClearingHandlers : PublishEndHtmlClearingHandlers
  {
    protected override string EventName { get; } = "publish:end:remote";

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }
  }
}