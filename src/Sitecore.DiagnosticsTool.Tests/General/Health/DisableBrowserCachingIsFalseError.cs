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
  public class DisableBrowserCachingIsFalseError : KbTest
  {
    public override string KbNumber => "394385";

    public override string KbName { get; } = "Sitecore Client applications may fail when \"DisableBrowserCaching\" is set to \"false\"";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var ex = log.Exception;
        if (ex == null)
        {
          return;
        }

        if (ex.Message.Contains("Object reference not set to an instance of an object.") && ex.StackTrace.Contains("Sitecore.Shell.Applications.ContentManager.ContentEditorForm.OnLoad(EventArgs e)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}