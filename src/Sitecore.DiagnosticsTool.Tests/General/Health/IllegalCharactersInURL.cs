namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class IllegalCharactersInUrl : KbTest
  {
    public override string KbNumber => "059908";

    public override string KbName { get; } = "Errors when using illegal characters in the URL";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 722;
    }

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

        if (ex.Message.Contains("Illegal characters in path.") && (ex.StackTrace.Contains("System.IO.Path.CheckInvalidPathChars(String path)") || ex.StackTrace.Contains("System.IO.Path.GetExtension(String path)")))
        {
          Report(output);
          return;
        }
      }
    }
  }
}