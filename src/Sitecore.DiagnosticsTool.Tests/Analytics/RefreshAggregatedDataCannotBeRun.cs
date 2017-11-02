// <summary>
//   http://issues.pssbuild1dk1.dk.sitecore.net/issue/SDT-123
// </summary>

namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class RefreshAggregatedDataCannotBeRun : KbTest
  {
    public override string KbNumber => "781372";

    public override string KbName { get; } = "Refresh Aggregated Data in Reports cannot be run with minimal SQL permissions";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var ex = log.Exception;
        if (ex == null)
        {
          return;
        }

        if (ex.Message.Contains("Cannot find the object \"Cache_TrafficByDay\" because it does not exist or you do not have permissions."))
        {
          Report(output);
          return;
        }
      }
    }
  }
}