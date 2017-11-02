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
  public class TrackingFieldAccess : KbTest
  {
    public override string KbNumber => "000050";

    public override string KbName { get; } = "Application Access Denied error when viewing the Tracking field";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var ex = log?.Exception;
        if (ex == null)
        {
          return;
        }

        if (ex.Message.Contains("Application access denied.") && ex.StackTrace.Contains("Sitecore.Shell.Applications.Analytics.TrackingField.TrackingFieldDetailsPage.OnLoad(EventArgs e)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}