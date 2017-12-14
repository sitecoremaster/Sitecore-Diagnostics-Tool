namespace Sitecore.DiagnosticsTool.Tests.General.Performance
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
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;

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

      var info = data.SitecoreInfo;
      var defaults = info.SitecoreDefaults;
      var defaultCachesPerDatabase = new Map<Map<CacheSizeDetails>>();
      var belowDefaultCachesPerDatabase = new Map<Map<CacheSizeDetails>>();
      foreach (var database in info.GetDatabases().Values)
      {
        if (database.Name == "filesystem")
        {
          continue;
        }

        var defaultDatabase = defaults.GetDatabases().TryGetValue(database.Name);
        if (defaultDatabase == null)
        {
          continue;
        }

        Process(info, output, database, defaultDatabase, defaultCachesPerDatabase, belowDefaultCachesPerDatabase);
      }

      if (defaultCachesPerDatabase.Any())
      {
        var message = "One or several Sitecore caches are not tuned up and use default settings which may lead to performance degradation";

        output.Warning(message, detailed: GetMessage(defaultCachesPerDatabase));
      }

      if (belowDefaultCachesPerDatabase.Any())
      {
        var message = "One or several Sitecore caches are use custom configuration which is below the minimum recommended values (set up by default) which may lead to performance degradation.";

        output.Error(message, detailed: GetMessage(belowDefaultCachesPerDatabase));
      }
    }

    private void Process(ISitecoreInformationContext info, ITestOutputContext output, LogicalDatabaseDefinition database, LogicalDatabaseDefinition defaultDatabase, Map<Map<CacheSizeDetails>> defaultCachesPerDatabase, Map<Map<CacheSizeDetails>> belowDefaultCachesPerDatabase)
    {
      var databaseCaches = new Map<CacheSizeDetails>();
      var belowDefaultCaches = new Map<CacheSizeDetails>();
      var databaseName = database.Name;

      foreach (var cache in database.Caches.Values)
      {
        var cacheSize = cache.Size;
        output.Debug($"{databaseName}[{cache.Name}] = {cacheSize}");

        var defaultSize = defaultDatabase.Caches[cache.Name].Size;
        if (defaultSize == null)
        {
          // custom cache, or filesystem database
          continue;
        }

        if (cacheSize == null)
        {
          var fallbackSettingCacheSize = CacheSize.Parse(info.GetSetting(cache.FallbackSettingName), info.SitecoreVersion);
          if (fallbackSettingCacheSize.Value.Bytes < defaultSize.Value.Bytes)
          {
            var cacheSizeDetails = new CacheSizeDetails
            {
              Value = fallbackSettingCacheSize,
              Comment = $"cache size is not specified, uses {cache.FallbackSettingName} setting value as fallback"
            };

            belowDefaultCaches.Add(cache.Name, cacheSizeDetails);
          }
        }
        else
        {
          if (cacheSize.Value == defaultSize.Value)
          {
            var cacheSizeDetails = new CacheSizeDetails
            {
              Value = cacheSize,
              Comment = "cache size is default"
            };

            databaseCaches.Add(cache.Name, cacheSizeDetails);
          }
          else if (cacheSize.Value < defaultSize.Value)
          {
            var cacheSizeDetails = new CacheSizeDetails
            {
              Value = cacheSize,
              Comment = $"cache size is below than default: {defaultSize.Value}"
            };

            belowDefaultCaches.Add(cache.Name, cacheSizeDetails);
          }
        }
      }

      if (databaseCaches.Any())
      {
        defaultCachesPerDatabase.Add(databaseName, databaseCaches);
      }

      if (belowDefaultCaches.Any())
      {
        belowDefaultCachesPerDatabase.Add(databaseName, belowDefaultCaches);
      }
    }

    protected DetailedMessage GetMessage(Map<Map<CacheSizeDetails>> result)
    {
      var rows = result
        .Where(x => x.Value.Any())
        .SelectMany(d =>
          d.Value.Select(c =>
            new TableRow(
              new Pair("Cache", $"{d.Key}[{c.Key}]"),
              new Pair("Size", c.Value.Value.Value.ToString()),
              new Pair("Comment", c.Value.Comment)
            )))
        .ToArray();

      var message = new DetailedMessage(
          new Table(rows),
          new Text("Read more in CMS Performance Tuning Guide on how to adjust cache settings."));

      return message;
    }

    public class CacheSizeDetails
    {
      public CacheSize Value { get; set; }

      public string Comment { get; set; } = "";
    }
  }
}