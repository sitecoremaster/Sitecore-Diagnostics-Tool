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
    public override string KbNumber { get; } = "135940";

    public override string KbName { get; } = "Session timeout when saving Rich Text fields";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ITestResourceContext data) 
      => data.SitecoreInfo.IsAnalyticsEnabled;

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var pipeline = data.SitecoreInfo.GetPipeline("pipelines/initializeTracker");
      var majorMinor = data.SitecoreInfo.SitecoreVersion.MajorMinorInt;
      if (majorMinor > 71)
      {
        return;
      }

      var patchedAssembly = AssemblyRef.Parse("Sitecore.Support.387512");
      var patchedType = TypeRef.Parse("Sitecore.Support.Analytics.Pipelines.InitializeTracker.Robots", patchedAssembly);
      if (pipeline.Processors.All(x => x.Type != patchedType))
      {
        Report(output);
      }
    }
  }
}