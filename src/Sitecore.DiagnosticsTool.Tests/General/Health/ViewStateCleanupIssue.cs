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
  public class ViewStateCleanupIssue : KbTest
  {
    public override string KbNumber => "288800";

    public override string KbName { get; } = "ViewState cleanup issue";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
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

        var stackTrace = ex.StackTrace;
        if (stackTrace == null)
        {
          continue;
        }

        var message = ex.Message;
        Assert.IsNotNull(message, "message");

        if (message.Contains("Value cannot be null.") && stackTrace.Contains("XamlPageStatePersister.Load()"))
        {
          Report(output);

          return;
        }

        var arr = new[]
        {
          "Sitecore.Shell.Applications.ContentManager.ContentEditorForm.OnLoad(EventArgs e)",
          "Sitecore.Shell.Applications.ShellForm.OnLoad(EventArgs e)"
        };

        if (message.Contains("Object reference not set to an instance of an object.") && arr.Any(x => stackTrace.Contains(x)))
        {
          Report(output);

          return;
        }
      }
    }
  }
}