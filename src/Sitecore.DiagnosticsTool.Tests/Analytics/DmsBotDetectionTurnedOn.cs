// <summary>
//   http://issues.pssbuild1dk1.dk.sitecore.net/issue/SDT-90
// </summary>

namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, looks very reasonable)
  [UsedImplicitly]
  public class DmsBotDetectionTurnedOn : Test
  {
    protected const string ErrorMessage = "Analytics bots detection is disabled, this may lead to excessive database growth.";
    
    public override string Name { get; } = "Bot detection must be turned on";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    public override bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var configuration = data.SitecoreInfo.Configuration;
      var excludeRobots = configuration.SelectSingleNode("/configuration/sitecore/analyticsExcludeRobots");
      if (excludeRobots == null)
      {
        output.Warning(ErrorMessage);
      }
    }
  }
}