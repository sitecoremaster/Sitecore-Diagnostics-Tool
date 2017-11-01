namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, looks valid)
  [UsedImplicitly]
  public class VisitInformationMayNotBeAvailableWhenEvaluatingConditionsInEngagementPlan : KbTest
  {
    public override string KbNumber => "784521";

    public override string KbName { get; } = "Visit information may not be available when evaluating conditions in Engagement Plan";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt < 75;
    }

    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var pipeline = data.SitecoreInfo.GetPipeline("pipelines/initialize");
      Assert.IsNotNull(pipeline);

      var supportTypeRef = TypeRef.Parse("Sitecore.Support.Analytics.Pipelines.Loader.InitializeAutomation", AssemblyRef.Parse("Sitecore.Support.381981.401128"));
      if (pipeline.Processors.All(x => x.Type != supportTypeRef))
      {
        Report(output);
      }
    }
  }
}