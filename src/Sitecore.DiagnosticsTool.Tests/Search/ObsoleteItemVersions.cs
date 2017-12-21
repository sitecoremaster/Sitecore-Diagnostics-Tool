namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class ObsoleteItemVersions : KbTest
  {
    protected const string OnPublishEndAsynchronousStrategy = @"Sitecore.ContentSearch.Maintenance.Strategies.OnPublishEndAsynchronousStrategy,Sitecore.ContentSearch";

    public override string KbName { get; } = "Obsolete item versions may be present in search indexes after publishing";

    public override string KbNumber => "992608";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 820;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var config = data.SitecoreInfo.Configuration;

      var indexes = config.SelectSingleNode(@"/configuration/sitecore/contentSearch/configuration/indexes");
      var strategies = new List<string>();
      foreach (XmlElement index in indexes)
      {
        var strategiesNode = index.SelectNodes(@"./strategies/strategy");
        if (strategiesNode == null)
        {
          continue;
        }

        foreach (XmlElement strategy in strategiesNode)
        {
          strategies.Add(strategy.GetAttribute("ref"));
        }
      }

      foreach (var strategyRef in strategies)
      {
        var strategy = config.SelectSingleNode($@"/configuration/sitecore/{strategyRef}") as XmlElement;
        if (!strategy.GetAttribute("type").Replace(" ", string.Empty).Equals(OnPublishEndAsynchronousStrategy, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        Report(output);

        return;
      }
    }
  }
}