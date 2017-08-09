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
  public class PathInfoInUrlError : KbTest
  {
    public override string KbNumber => "751574";

    public override string KbName { get; } = "Errors when the URL includes path info";

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

        if (ex.Message.Contains("Length cannot be less than zero.") && ex.StackTrace.Contains("Sitecore.Pipelines.HttpRequest.SiteResolver.Process(HttpRequestArgs args)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}