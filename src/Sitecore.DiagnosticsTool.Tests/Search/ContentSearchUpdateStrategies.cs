namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-07-31, fixed)
  [UsedImplicitly]
  public class ContentSearchUpdateStrategies : Test
  {
    public override string Name { get; } = "ContentSearch update strategies point to correct databases";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.SearchIndexing};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var config = data.SitecoreInfo.Configuration;
      var sitecore = config.SelectSingleNode("/configuration/sitecore");
      if (sitecore == null)
      {
        return;
      }

      var indexes = config.SelectElements(@"/configuration/sitecore/contentSearch/configuration/indexes/index");
      foreach (var index in indexes)
      {
        if (index == null)
        {
          continue;
        }

        var indexDatabases = index.SelectElements("locations/*/Database").Select(x => x.InnerText).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
        if (!indexDatabases.Any())
        {
          continue;
        }

        var indexName = index.GetAttribute("id");
        var indexStrategiesXPaths = index.SelectElements("strategies/strategy").Select(x => x.GetAttribute("ref")).Distinct().ToArray();
        foreach (var strategyXPath in indexStrategiesXPaths)
        {
          if (string.IsNullOrEmpty(strategyXPath))
          {
            output.Debug(GetIndexStrategiesCorruptedMessage(indexName));
            break;
          }

          var strategy = sitecore.SelectSingleNode(strategyXPath);
          if (strategy == null)
          {
            output.Debug(GetIndexStrategiesCorruptedMessage(indexName));
            continue;
          }

          var strategyDatabase = strategy.SelectSingleNode("param[@desc='database']").With(x => x.InnerText);

          if (strategy.Name.Equals("manual", StringComparison.InvariantCultureIgnoreCase))
          {
            continue;
          }

          if (string.IsNullOrEmpty(strategyDatabase))
          {
            output.Debug(GetStrategyCorruptedMessage(strategy.Name));
            continue;
          }

          if (!indexDatabases.Any(x => x.Equals(strategyDatabase)))
          {
            output.Error(GetErrorMessage(indexName, string.Join(", ", indexDatabases), strategy.Name, strategyDatabase));
          }
        }
      }
    }

    [NotNull]
    protected static string GetIndexStrategiesCorruptedMessage([NotNull] string indexName)
    {
      Assert.ArgumentNotNull(indexName, nameof(indexName));

      return $"The configuration of {indexName} index strategies look suspicious. It is recommended to review them.";
    }

    [NotNull]
    protected static string GetStrategyCorruptedMessage([NotNull] string strategy)
    {
      Assert.ArgumentNotNull(strategy, nameof(strategy));

      return $"The configuration of the {strategy} strategy looks suspicious. It is recommended to review it.";
    }

    [NotNull]
    protected static string GetErrorMessage([NotNull] string index, [NotNull] string indexDatabase, [NotNull] string strategy, [NotNull] string strategyDatabase)
    {
      Assert.ArgumentNotNull(index, nameof(index));
      Assert.ArgumentNotNull(indexDatabase, nameof(indexDatabase));
      Assert.ArgumentNotNull(strategy, nameof(strategy));
      Assert.ArgumentNotNull(strategyDatabase, nameof(strategyDatabase));

      return $"The index {index} uses the {indexDatabase} database, but the update strategy {strategy} is associated with {strategyDatabase} database which looks suspicious. It is recommended to review it.";
    }
  }
}