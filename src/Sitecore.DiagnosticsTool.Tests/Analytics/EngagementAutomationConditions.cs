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
  public class EngagementAutomationConditions : KbTest
  {
    protected const string ErrorMessage = "'Sitecore.Rules.ConditionalRenderings.ConditionalRenderingsRuleContext', on 'Sitecore.Analytics.Rules.Workflows.Conditions.HasAutomationCondition`1[T]' violates the constraint of type parameter 'T'";

    public override string KbNumber => "117761";

    public override string KbName { get; } = "Issue with using Engagement Automation conditions in personalization";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 75;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(ErrorMessage)))
      {
        Report(output);
      }
    }
  }
}