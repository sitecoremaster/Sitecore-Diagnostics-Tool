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
  public class ImageValidatorErrors : KbTest
  {
    public override string KbNumber => "043860";

    public override string KbName { get; } = "Image validators may cause exceptions in logs";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var message = log.Message;
        var ex = log.Exception;
        if (ex == null)
        {
          return;
        }

        if (message.Contains("Attempted to load invalid xml.") && ex.Message.Contains("Data at the root level is invalid"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}