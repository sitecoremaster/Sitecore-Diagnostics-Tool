namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ShowWarningIfTheRobotDetectionIsEnabled : Test
  {
    [UsedImplicitly]
    public ShowWarningIfTheRobotDetectionIsEnabled()
    {
    }

    protected const string ErrorMessage = "Sitecore automatic robot detection functionality is disabled. This functionality filters out unwanted interactions from automated browsers and robots. Sitecore recommends enabling 'Analytics.AutoDetectBots' setting to reduce the growth of the xDB";

    public override string Name => "Sitecore robot detection functionality is disabled.";

    public override IEnumerable<Category> Categories => new[] { Category.Analytics };

    protected override bool IsActual(IInstanceResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      if (!data.SitecoreInfo.GetBoolSetting("Analytics.AutoDetectBots"))
      {
        output.Warning(ErrorMessage);
      }
    }
  }
}