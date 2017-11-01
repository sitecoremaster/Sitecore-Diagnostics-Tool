namespace Sitecore.DiagnosticsTool.Tests.WFFM
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
  public class WffmDoubledBrackets : KbTest
  {
    protected const string Message = "The specified string is not in the form required for an e-mail address.";

    public override string KbNumber => "785584";

    public override string KbName { get; } = "Error submitting form when using Send Email action in WFFM";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Wffm};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Warn);
      if (logs.Any(log => log.RawText.Contains(Message)))
      {
        Report(output);
      }
    }
  }
}