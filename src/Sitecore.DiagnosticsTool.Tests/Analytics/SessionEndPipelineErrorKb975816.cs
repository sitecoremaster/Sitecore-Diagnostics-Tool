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
  public class SessionEndPipelineErrorKb975816 : KbTest
  {
    protected const string PipelineName = "pipelines/createTracker";

    protected const string ErrorText = "SessionEndPipeline";

    public override string KbNumber => "975816";

    public override string KbName { get; } = "SessionEndPipeline failed errors appear in the log when Analytics tracking is disabled";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var info = data.SitecoreInfo;
      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);

      // "The following error may appear in the Sitecore XP log when the Sitecore.Analytics.Tracking.config file is disabled"
      var pipeline = info.GetPipeline(PipelineName);
      if (pipeline == null)
      {
        if (logs.Any(log => log.RawText.Contains(ErrorText)))
        {
          Report(output);
        }
      }
    }
  }
}