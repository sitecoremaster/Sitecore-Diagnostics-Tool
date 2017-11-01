namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class MediaLinkPrefixContainsTilde : KbTest
  {
    public override string KbNumber => "723979";

    public override string KbName { get; } = "Tilde character in Media Prefix";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Performance};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var info = data.SitecoreInfo;
      var mediaPrefix = info.GetSetting("Media.MediaLinkPrefix");
      if (string.IsNullOrEmpty(mediaPrefix) && info.SitecoreVersion.MajorMinorInt < 81) // before 81 default value was ~/media
      {
        Report(output);
        return;
      }

      if (mediaPrefix.Contains("~"))
      {
        Report(output);
      }
    }
  }
}