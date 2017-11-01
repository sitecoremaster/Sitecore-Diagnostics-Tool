namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class CrawlerRootItemIgnored : KbTest
  {
    public override string KbName { get; } = "The crawler root item is ignored on search index updates";

    public override string KbNumber => "505785";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.SearchIndexing};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 722;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      var configuration = data.SitecoreInfo.Configuration;
      var indexes = configuration.SelectNodes("/configuration/sitecore/contentSearch/configuration/indexes/index");
      if (indexes == null)
      {
        return;
      }

      foreach (XmlNode index in indexes)
      {
        var crawler = index?.SelectSingleNode("locations/crawler") as XmlElement;
        if (crawler == null || crawler.GetAttribute("type").Contains("406670"))
        {
          continue;
        }

        var crawlerRoot = index.SelectSingleNode("locations/crawler/Root") as XmlElement;
        if (crawlerRoot == null || crawlerRoot.InnerText == "/sitecore")
        {
          continue;
        }

        Report(output);

        break;
      }
    }
  }
}