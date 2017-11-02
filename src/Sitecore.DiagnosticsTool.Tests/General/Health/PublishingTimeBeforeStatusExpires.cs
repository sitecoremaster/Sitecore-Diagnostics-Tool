namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class PublishingTimeBeforeStatusExpires : KbTest
  {
    public override string KbNumber => "908303";

    public override string KbName { get; } = "Publishing may be interrupted due to the value of the Publishing.TimeBeforeStatusExpires setting";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 722;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      var entries = data.Logs.GetSitecoreLogEntries(LogLevel.Error);

      var isActual = entries
        .Select(entry => entry
          .With(x => x.Exception)
          .With(x => x.InnerException)
          .With(x => x.InnerException))
        .Any(x => x != null && x.Message.Equals("The publishing process was unexpectedly interrupted.") && x.StackTrace.Trim().Equals(@"Sitecore.Client at Sitecore.Shell.Applications.Dialogs.Publish.PublishForm.CheckStatus()"));

      if (!isActual)
      {
        return;
      }

      Report(output);
    }
  }
}