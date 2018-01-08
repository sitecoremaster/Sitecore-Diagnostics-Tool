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
  public class WffmObsoleteCommands : KbTest
  {
    protected const string Message1 = "Could not instantiate \"Sitecore.Forms.Core.Commands.View.Refresh,Sitecore.Forms.Core\" command object.";

    protected const string Message2 = "Could not instantiate \"Sitecore.Forms.Core.Commands.Fields.SelectFields,Sitecore.Forms.Core\" command object.";

    protected const string Message3 = "Could not instantiate \"Sitecore.Form.Core.Commands.OpenSession,Sitecore.Forms.Core\" command object.";

    public override string KbNumber => "878954";

    public override string KbName { get; } = "WFFM errors due to obsolete commands in the Sitecore.Forms.config file.";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Wffm };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major == 8;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      foreach (var log in logs)
      {
        var ex = log.RawText;
        if (ex.Contains(Message1) || ex.Contains(Message2) || ex.Contains(Message3))
        {
          Report(output);

          return;
        }
      }
    }
  }
}