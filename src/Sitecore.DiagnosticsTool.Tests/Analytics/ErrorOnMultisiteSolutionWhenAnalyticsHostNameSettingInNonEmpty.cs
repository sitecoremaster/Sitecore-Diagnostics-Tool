namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class ErrorOnMultisiteSolutionWhenAnalyticsHostNameSettingInNonEmpty : KbTest
  {
    public override string KbNumber => "792020";

    public override string KbName { get; } = "Error on multisite solution when Analytics.HostName setting is non-empty";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 75;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      const string Exception = "The remote name could not be resolved: 'http'";

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(Exception)))
      {
        Report(output);
      }
    }
  }
}