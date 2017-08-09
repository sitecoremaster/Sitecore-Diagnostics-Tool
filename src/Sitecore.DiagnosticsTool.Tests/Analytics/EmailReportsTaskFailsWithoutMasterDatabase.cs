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
  public class EmailReportsTaskFailsWithoutMasterDatabase : KbTest
  {
    public override string KbNumber => "897224";

    public override string KbName { get; } = "EmailReportsTask fails without Master database";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };
    
    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      const string NoMaster = "Could not find configuration node: databases/database[@id='master']";
      const string EmailReportsTask = "Sitecore.Analytics.Tasks.EmailReportsTask.GetLastRun";

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(NoMaster) && log.RawText.Contains(EmailReportsTask)))
      {
        Report(output);
      }
    }
  }
}