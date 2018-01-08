namespace Sitecore.DiagnosticsTool.Tests.WFFM
{
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class WffmBusinessName : KbTest
  {
    protected const string FormsDataProvider80 = "Sitecore.WFFM.Analytics.Providers.AnalyticsDataProvider, Sitecore.WFFM.Analytics";

    protected const string FormsDataProvider81 = "Sitecore.WFFM.Analytics.Providers.AnalyticsFormsDataProvider, Sitecore.WFFM.Analytics";

    public override string KbNumber => "505444";

    public override string KbName { get; } = "Business name is used instead of contact name in WFFM reports";

    public override IEnumerable<Category> Categories { get; } = new[]
    {
      Category.Wffm
    };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt == 80 || sitecoreVersion.MajorMinorInt == 81;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      if (data.SitecoreInfo.SitecoreVersion.Minor == 0)
      {
        if (CheckConfigurationForError("/configuration/sitecore/wffm/formsDataProvider", true, data))
        {
          Report(output);
        }
      }
      else
      {
        if (CheckConfigurationForError("/configuration/sitecore/wffm/analytics/formsDataProvider", false, data))
        {
          Report(output);
        }
      }
    }

    private static bool CheckConfigurationForError(string nodePath, bool is8Version, IInstanceResourceContext data)
    {
      var dataProvider = data.SitecoreInfo.Configuration.SelectSingleNode(nodePath) as XmlElement;
      var addError = false;
      if (!is8Version)
      {
        var xmlElement =
          data.SitecoreInfo.Configuration.SelectSingleNode(
            "/configuration/sitecore/wffm/analytics/analyticsFormsDataProvider") as XmlElement;
        if (xmlElement != null && dataProvider != null && dataProvider.GetAttribute("ref").Contains("analyticsFormsDataProvider") && xmlElement.GetAttribute("type").Equals(FormsDataProvider81))
        {
          addError = true;
        }
      }
      else
      {
        if (dataProvider != null && dataProvider.GetAttribute("type").Equals(FormsDataProvider80))
        {
          addError = true;
        }
      }

      return addError;
    }
  }
}
