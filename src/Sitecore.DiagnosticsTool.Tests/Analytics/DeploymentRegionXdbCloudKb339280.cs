namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class DeploymentRegionXdbCloudKb339280 : KbTest
  {
    protected const string ErrorText = "The setting for Sitecore.Cloud.Xdb.DeploymentRegion was not configured";

    public override string KbNumber => "339280";

    public override string KbName { get; } = "Deployment region must be configured in Sitecore xDB Cloud";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(ErrorText)))
      {
        Report(output);
      }
    }
  }
}