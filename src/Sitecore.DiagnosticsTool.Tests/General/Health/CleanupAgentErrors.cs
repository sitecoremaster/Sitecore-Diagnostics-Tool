namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class CleanupAgentErrors : KbTest
  {
    public override string KbNumber => "113648";

    public override string KbName { get; } = "Errors in Sitecore Cleanup Agent";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
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

        var message = log.Message;
        if (message.Contains("Exception in Scheduling.CleanupAgent. Folder:"))
        {
          // && ex.Type == "System.UnauthorizedAccessException")
          Report(output);
          return;
        }
      }
    }
  }
}