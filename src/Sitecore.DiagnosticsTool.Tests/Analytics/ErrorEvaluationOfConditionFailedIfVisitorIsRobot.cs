namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, looks ok)
  [UsedImplicitly]
  public class ErrorEvaluationOfConditionFailedIfVisitorIsRobot : KbTest
  {
    public override string KbNumber => "960279";

    public override string KbName { get; } = "'Evaluation of condition failed' errors appear when the website visitors are identified as robots";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 75;
    }

    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var pipeline = data.SitecoreInfo.GetPipeline("pipelines/createTracker");
      Assert.IsNotNull(pipeline);

      var supportTypeRef75_80 = TypeRef.Parse("Sitecore.Support.Analytics.Pipelines.CreateTracker.GetTracker", AssemblyRef.Parse("Sitecore.Support.424667"));
      var supportTypeRef81 = TypeRef.Parse("Sitecore.Support.Analytics.Pipelines.CreateTracker.GetTracker", AssemblyRef.Parse("Sitecore.Support.424667.81"));

      var processor = pipeline.Processors.FirstOrDefault(proc => proc.Type == supportTypeRef75_80 || proc.Type == supportTypeRef81);
      var patch = data.SitecoreInfo.Assemblies.ContainsKey("Sitecore.Support.424667".ToLower());
      if (!patch || processor == null)
      {
        Report(output);
      }
    }
  }
}