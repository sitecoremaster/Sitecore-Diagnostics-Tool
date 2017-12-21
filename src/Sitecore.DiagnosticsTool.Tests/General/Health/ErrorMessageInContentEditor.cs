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
  public class ErrorMessageInContentEditor : KbTest
  {
    public override string KbNumber => "384386";

    public override string KbName { get; } = "Error message in the Content Editor";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var message = log.Message;
        if (message.Contains("Multiple controls with the same ID '") && message.Contains("' were found. FindControl requires that controls have unique IDs."))
        {
          Report(output);
          return;
        }
      }
    }
  }
}