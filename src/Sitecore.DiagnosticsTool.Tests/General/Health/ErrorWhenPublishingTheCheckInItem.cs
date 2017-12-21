namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class ErrorWhenPublishingTheCheckInItem : KbTest
  {
    protected const string Message = "The incorrect template configured for for the /sitecore/system/Settings/Workflow/Check in item in the Master and Core databases.";

    protected const string Pattern = @"Cannot find dictionary domain for the dictionaty entry '/sitecore/system/Settings/Workflow/Check in'";

    public override string KbNumber => "745899";

    public override string KbName { get; } = "Error when publishing the Check In item";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(x => x != null && x.Message.Contains(Pattern)))
      {
        output.Error(Message, Link);
      }
    }
  }
}