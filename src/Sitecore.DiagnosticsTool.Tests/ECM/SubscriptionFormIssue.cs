namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class SubscriptionFormIssue : KbTest
  {
    public override string KbName { get; } = "Parser error in the Subscription form control";

    public override string KbNumber => "363521";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt == 80;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var ecmVersion = EcmHelper.GetEcmVersion(data);
      if (ecmVersion == null || ecmVersion.Major != 3 || ecmVersion.Minor != 1)
      {
        return;
      }

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      var errorFound = logs.Any(entry => entry.Exception.Message.Contains("Could not load type 'Sitecore.Modules.EmailCampaign.Layouts.SubscriptionForm'"));
      if (errorFound)
      {
        Report(output);
      }
    }
  }
}