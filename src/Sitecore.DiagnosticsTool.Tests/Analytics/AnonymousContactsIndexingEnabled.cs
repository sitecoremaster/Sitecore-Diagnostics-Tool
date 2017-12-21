namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class AnonymousContactsIndexingEnabled : KbTest
  {
    protected string SettingName = "ContentSearch.Analytics.IndexAnonymousContacts";

    [UsedImplicitly]
    public AnonymousContactsIndexingEnabled()
    {
    }

    public override string KbNumber => "171238";

    public override string KbName { get; } = "Issues caused by indexing anonymous contacts";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt >= 812;
    }

    protected override bool IsActual(IInstanceResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.SitecoreInfo.GetBoolSetting(SettingName, true))
      {
        Report(output);
      }
    }
  }
}