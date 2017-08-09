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
  public class EmailReportsTaskWithoutMasterDb : KbTest
  {
    public override string KbNumber => "897224";

    public override string KbName { get; } = "EmailReportsTask fails without Master database";

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
          continue;
        }

        var nested = ex.InnerException;
        if (nested == null)
        {
          return;
        }

        if (nested.Message.Contains("Could not find configuration node: databases/database[@id='master']") && nested.StackTrace.Contains("Sitecore.Analytics.Tasks.EmailReportsTask.GetLastRun()"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}