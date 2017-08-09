namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class PublishingIssue : KbTest
  {
    public override string KbName { get; } = "Message links and statistics are not available in ECM when dispatching messages";

    public override string KbNumber => "173546";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    [NotNull]
    protected string ErrorMessage => "Message links and statistics may not be available in ECM after dispatch. Please refer to the following article for more information";

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt < 75;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (!data.SitecoreInfo.Assemblies.ContainsKey("Sitecore.Support.388644.dll".ToLower()))
      {
        Report(output, ErrorMessage);
      }
    }
  }
}