namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class MultipleSolrCoresHttpCache : KbTest
  {
    protected const string WarningMsg = "You may experience some issues with the Content Search functionality.";

    public override string KbNumber => "810499";

    public override string KbName { get; } = "Issues with multiple SOLR cores and the EnableHttpCache setting";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.SearchIndexing};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorUpdateInt < 751 || sitecoreVersion.Major == 8 && sitecoreVersion.MajorMinorUpdateInt < 802;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var enableHttpCache = data.SitecoreInfo.GetBoolSetting("ContentSearch.Solr.EnableHttpCache");
      if (!enableHttpCache)
      {
        return;
      }

      var indexes = data.SitecoreInfo.ContentSearchIndexes.Values;

      var cores = new List<string>(indexes.Count());
      foreach (var index in indexes)
      {
        var core = index.Configuration.SelectElements(@"//param[@desc='core']").First();
        cores.Add(core.InnerText);
      }

      cores = cores.Distinct().ToList();
      var count = cores.Count;
      if (count == 0 || count == 1 && cores[0] != "$(id)")
      {
        return;
      }

      var globalAsaxFile = data.SitecoreInfo.GlobalAsaxFile;

      if (globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.NinjectIntegration.NinjectApplication\"")
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.StructureMapIntegration.StructureMapApplication\"")
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.UnityIntegration.UnityApplication\""))
      {
        output.Warning(WarningMsg, url: Link);
      }
    }
  }
}