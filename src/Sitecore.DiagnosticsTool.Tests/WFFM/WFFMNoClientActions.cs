namespace Sitecore.DiagnosticsTool.Tests.WFFM
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class WffmNoClientActions : KbTest
  {
    protected const string Message = "form has no actions";

    public override string KbNumber => "915372";

    public override string KbName { get; } = "\"The form has no actions\" warning is added to the log if no client actions were found";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Wffm };

    public override IEnumerable<ServerRole> ServerRoles => new[] { ServerRole.ContentDelivery };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Warn);
      foreach (var log in logs)
      {
        var ex = log.RawText;
        if (ex.Contains(Message))
        {
          Report(output);

          return;
        }
      }
    }
  }
}