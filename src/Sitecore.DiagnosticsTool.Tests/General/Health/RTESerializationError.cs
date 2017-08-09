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
  public class RteSerializationError : KbTest
  {
    public override string KbNumber => "660104";

    public override string KbName { get; } = "\"Cannot deserialize dialog parameters\" error in the Rich Text Editor";

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

        if (ex.Message.Contains("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters."))
        {
          Report(output);
          return;
        }
      }
    }
  }
}