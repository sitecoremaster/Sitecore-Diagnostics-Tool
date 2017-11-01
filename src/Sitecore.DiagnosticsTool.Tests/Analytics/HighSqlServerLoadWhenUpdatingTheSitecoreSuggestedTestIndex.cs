namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class HighSqlServerLoadWhenUpdatingTheSitecoreSuggestedTestIndex : KbTest
  {
    public override string KbNumber => "028657";

    public override string KbName { get; } = "High SQL Server load when updating the sitecore_suggested_test_index";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 806;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);

      var condition1 = false;
      var condition2 = false;

      const string Exception = "The timeout period elapsed prior to obtaining a connection from the pool.";
      const string Info = "Starting: Rebuild Suggested Tests Index";

      foreach (var log in logs)
      {
        var ex = log.RawText;
        if (!condition1 && ex.Contains(Exception))
        {
          condition1 = true;
        }

        if (!condition2 && ex.Contains(Info))
        {
          condition2 = true;
        }

        if (condition1 && condition2)
        {
          Report(output);

          return;
        }
      }
    }
  }
}