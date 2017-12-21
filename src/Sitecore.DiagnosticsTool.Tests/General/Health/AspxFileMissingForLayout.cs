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
  public class AspxFileMissingForLayout : KbTest
  {
    public override string KbNumber => "552607";

    public override string KbName { get; } = "Errors when an .ASPX file for a layout is missing";

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
          return;
        }

        // ex.Type == "System.Web.HttpException" && 
        if (ex.Message.Contains("The file '") && ex.Message.Contains("' does not exist.") && ex.StackTrace.Contains("System.Web.UI.Util.CheckVirtualFileExists(VirtualPath virtualPath)") && ex.StackTrace.Contains("System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}