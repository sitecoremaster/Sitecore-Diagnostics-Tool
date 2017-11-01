namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class ExceptionIfLongDataIsStoredInCollectionDB : KbTest
  {
    public override string KbNumber => "042561";

    public override string KbName { get; } = "Exception may be thrown while saving results to the reporting database if the data is too long";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt >= 804;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var pipeline = data.SitecoreInfo.GetPipeline("pipelines/interactions");
      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      var exception = "Exception when storing an aggregation result into reporting database";
      var exceptionMessage = "String or binary data would be truncated";
      if (pipeline == null)
      {
        return;
      }

      var supportTypeRef = TypeRef.Parse("Sitecore.Support.Analytics.Aggregation.Pipeline.TruncateLongDataProcessor", AssemblyRef.Parse("Sitecore.Support.97934"));
      var processor = pipeline.Processors.FirstOrDefault(proc => proc.Type == supportTypeRef);
      if (logs.Any(log => log.RawText.Contains(exception)) && logs.Any(log => log.RawText.Contains(exceptionMessage)))
      {
        if (!data.SitecoreInfo.Assemblies.ContainsKey("Sitecore.Support.97934".ToLower()) || processor == null)
        {
          Report(output);
        }
      }
    }
  }
}