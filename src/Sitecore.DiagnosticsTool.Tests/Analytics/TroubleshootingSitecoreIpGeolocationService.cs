namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class TroubleshootingSitecoreIpGeolocationService : KbTest
  {
    public override string KbNumber => "798498";

    public override string KbName { get; } = "Troubleshooting Sitecore IP Geolocation service";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      const string Exception = "Error retrieving geo ip information for ip";

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(Exception)))
      {
        Report(output);
      }
    }
  }
}