namespace Sitecore.DiagnosticsTool.Tests.WFFM
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
  public class WffmSubmitFormTwice : KbTest
  {
    protected const string Message = "The conversion of a datetime data type to a smalldatetime data type resulted in an out-of-range value";

    public override string KbNumber => "282795";

    public override string KbName { get; } = "Errors when a contact submits a WFFM form twice.";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Wffm};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt == 80;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      foreach (var log in logs)
      {
        var ex = log.RawText;
        if (ex.Contains(Message))
        {
          Report(output);

          return;
        }
      }
    }
  }
}