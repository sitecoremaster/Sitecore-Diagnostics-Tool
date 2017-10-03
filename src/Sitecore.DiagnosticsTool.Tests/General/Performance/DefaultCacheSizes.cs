namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.Extensions;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class DefaultCacheSizes : Test
  {
    protected const string PrefetchXPath = "dataProviders/dataProvider[@ref='dataProviders/main']/prefetch/cacheSize";

    [NotNull]
    protected readonly string[] DatabaseCacheNames =
    {
      "data",
      "items"
    };

    public override string Name { get; } = "Sitecore Cache configuration needs adjusting";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var configuration = data.SitecoreInfo.Configuration;
      var result = new Map<Map>();
      foreach (var database in configuration.GetDatabases())
      {
        Process(data, output, database, result);
      }

      if (result.Any())
      {
        var message = GetMessage(result);

        output.Warning(message);
      }
    }

    private void Process(ITestResourceContext data, ITestOutputContext output, XmlElement database, Map<Map> result)
    {
      var defaults = data.SitecoreInfo.SitecoreDefaults.Configuration;
      var outputCacheNames = new Map();
      var databaseName = database.GetAttribute("id");
      foreach (var cacheName in DatabaseCacheNames)
      {
        var cache = database.SelectSingleNode("cacheSizes/" + cacheName);
        if (cache == null)
        {
          continue;
        }

        var cacheSize = cache.InnerText;
        output.Debug($"Database {databaseName} {cacheName} cache size: {cacheSize}");

        var defaultSizeElement = defaults.SelectSingleNode(cache.GetXPath());
        Assert.IsNotNull(defaultSizeElement, "defaultSizeElement");

        var defaultSize = defaultSizeElement.InnerText;
        if (cacheSize == defaultSize)
        {
          outputCacheNames.Add(cacheName, cacheSize);
        }
      }

      var prefetchNode = database.SelectSingleNode(PrefetchXPath);
      if (prefetchNode != null)
      {
        var prefetchSize = prefetchNode.InnerText;
        if (!string.IsNullOrEmpty(prefetchSize))
        {
          prefetchSize = prefetchSize.Trim();
          output.Debug($"Database {databaseName} prefetch cache size: {prefetchSize}");
        }

        var defaultPrefetchNode = defaults.SelectSingleNode(prefetchNode.GetXPath());
        Assert.IsNotNull(defaultPrefetchNode, "defaultPrefetchNode: " + databaseName);

        var defaultPrefetch = defaultPrefetchNode.InnerText;
        if (!string.IsNullOrEmpty(defaultPrefetch))
        {
          defaultPrefetch = defaultPrefetch.Trim();
        }

        if (prefetchSize == defaultPrefetch)
        {
          outputCacheNames.Add("prefetch", prefetchSize);
        }
      }

      if (outputCacheNames.Any())
      {
        result.Add(databaseName, outputCacheNames);
      }
    }

    protected ShortMessage GetMessage(Map<Map> result)
    {
      var databases = result
        .Where(x => x.Value.Any())
        .Select(x => new Container(
          new BoldText(x.Key),
          new Text($" database caches need tuning:"),
          BulletedList.Create(x.Value, c => $"{c.Key} = {c.Value} (default)")))
        .ToArray();

      var message = new ShortMessage(
        new Text($"One or several Sitecore caches are not tuned up and use default settings which may lead to performance degradation:"),
        new BulletedList(databases),
        new Text("Read more in CMS Performance Tuning Guide on how to adjust cache settings."));

      return message;
    }
  }
}