namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class LuceneNoGo : SolutionTest
  {
    public override string Name { get; } = "Lucene in scaled setups";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.SearchIndexing};

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      var numberOfSearchEnabledInstances = data.Values.Count(x => x.SitecoreInfo.ContentSearchIndexes.Any());
      if (numberOfSearchEnabledInstances < 2)
      {
        // if total number of Sitecore instances that actually use search is 1 or 0, we don't need to offer Solr
        return;
      }

      var index2instances = new Map<List<string>>();
      foreach (var instance in data.Values)
      {
        var luceneIndexes = instance.SitecoreInfo.ContentSearchIndexes.Values.Where(i => i.SearchProvider == SearchProvider.Lucene).ToArray();
        foreach (var index in luceneIndexes)
        {
          var instances = index2instances.GetOrAdd(index.Id, new List<string>());
          Assert.IsNotNull(instances);

          instances.Add(instance.SitecoreInfo.InstanceName);
        }
      }

      if (index2instances.Any())
      {
        var rows = new List<TableRow>();
        foreach (var indexName in index2instances.Keys)
        {
          var instances = index2instances[indexName];
          rows.Add(new TableRow(new[] {new Pair("Index", indexName)}
            .Concat(instances.Select(z => new Pair(z, "Lucene")))));
        }

        var text = "The solution is configured to use Lucene search engine for the number of Content Search indexes listed below. It is highly " +
          "recommended to consider upgrading your solution to use Solr, as Lucene cannot deliver acceptable level of stability, consistency and maintainability.";

        var message = new ShortMessage(new Text(text));
        var detailed = new DetailedMessage(new Table(rows.ToArray()));

        output.Warning(message, detailed: detailed);
      }
    }
  }
}