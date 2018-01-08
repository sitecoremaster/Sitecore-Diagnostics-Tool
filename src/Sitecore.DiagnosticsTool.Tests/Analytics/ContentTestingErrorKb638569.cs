namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class ContentTestingErrorKb638569 : KbTest
  {
    protected readonly TypeRef GetContentEditorWarningsType = TypeRef.Parse(@"Sitecore.Support.ContentTesting.Pipelines.GetContentEditorWarnings.GetContentTestingWarnings,Sitecore.Support.431160");

    public override string KbName { get; } = "Content Testing errors in Solr and Sitecore logs when using Content Editor";

    public override string KbNumber => "638569";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing, Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var info = data.SitecoreInfo;

      var getContentEditorWarnings = info.GetPipeline("pipelines/getContentEditorWarnings");
      Assert.IsNotNull(getContentEditorWarnings, "getContentEditorWarnings");

      foreach (var processor in getContentEditorWarnings.Processors)
      {
        Assert.IsNotNull(processor, "processor");

        var type = processor.Type;
        if (type.Equals(GetContentEditorWarningsType))
        {
          return;
        }
      }

      var testingIndexName = data.SitecoreInfo.GetSetting(@"ContentTesting.TestingIndexName");
      if (!info.ContentSearchIndexes.ContainsKey(testingIndexName))
      {
        return;
      }

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      if (logs.Any(log => log.RawText.Contains(@"Cannot parse '(((__smallupdateddate_tdt")))
      {
        Report(output);
      }
    }
  }
}