namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: TODO (2017-06-12)
  [UsedImplicitly]
  public class SessionTimeoutWhenSavingRichTextFields : KbTest
  {
    public override string KbNumber => "135940";

    public override string KbName { get; } = "Session timeout when saving Rich Text fields";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 817;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.IsNotNull(data, "context");

      var pipeline = data.SitecoreInfo.GetPipeline("pipelines/initializeTracker");
      Assert.IsNotNull(pipeline);

      var version = data.SitecoreInfo.SitecoreVersion;
      var ver = version.MajorMinorUpdateInt;
      if (ver < 720)
      {
        var supportProcessor = TypeRef.Parse("Sitecore.Support.Analytics.Pipelines.InitializeTracker.Robots", AssemblyRef.Parse("Sitecore.Support.387512"));
        if (pipeline.Processors.All(x => x.Type != supportProcessor))
        {
          Report(output);
        }

      }
      else if (false
        || ver >= 720 && ver <  725 
        || ver >= 750 && ver <= 752
        || ver >= 800 && ver <  806
        || ver >= 810 && ver <  817
      )
      {
        var supportProcessor = TypeRef.Parse("Sitecore.Support.Analytics.RobotDetection.Pipelines.InitializeTracker.Robots", AssemblyRef.Parse("Sitecore.Support.414299"));
        if (pipeline.Processors.All(x => x.Type != supportProcessor))
        {
          Report(output);
        }
      }
    }
  }
}