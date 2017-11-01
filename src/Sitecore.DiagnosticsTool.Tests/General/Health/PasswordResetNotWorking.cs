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
  public class PasswordResetNotWorking : KbTest
  {
    public override string KbNumber => "730487";

    public override string KbName { get; } = "Membership provider password reset functionality does not work when question/answer option is enabled";

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

        if (ex.Message.Contains("Value cannot be null. Parameter name: passwordAnswer"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}