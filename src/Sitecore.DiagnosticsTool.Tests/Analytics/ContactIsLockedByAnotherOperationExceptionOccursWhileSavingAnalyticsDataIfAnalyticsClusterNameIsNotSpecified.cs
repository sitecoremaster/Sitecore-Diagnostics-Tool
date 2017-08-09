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
  public class ContactIsLockedByAnotherOperationExceptionOccursWhileSavingAnalyticsDataIfAnalyticsClusterNameIsNotSpecified : KbTest
  {
    public override string KbNumber => "965127";

    public override string KbName { get; } = @"""A contact is locked by another operation"" exception occurs while saving Analytics data if Analytics.ClusterName is not specified";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt < 75;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      const string Exception = "A contact is locked by another operation";

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(Exception)))
      {
        Report(output);
      }
    }
  }
}