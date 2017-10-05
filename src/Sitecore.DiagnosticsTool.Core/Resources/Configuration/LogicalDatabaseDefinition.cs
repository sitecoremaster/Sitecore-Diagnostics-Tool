namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;

  public class LogicalDatabaseDefinition
  {
    [NotNull] 
    private static readonly Map<CacheDefinition> CacheDefinitions = new[]
    {
      new CacheDefinition("data", "cacheSizes/data", "Caching.DefaultDataCacheSize"),
      new CacheDefinition("items", "cacheSizes/items", "Caching.DefaultItemCacheSize"),
      new CacheDefinition("paths", "cacheSizes/paths", "Caching.DefaultPathCacheSize"),
      new CacheDefinition("standardValues", "cacheSizes/standardValues", "Caching.StandardValues.DefaultCacheSize"),
      new CacheDefinition("itempaths", "cacheSizes/itempaths", "Caching.DefaultItemPathsCacheSize"),
      new CacheDefinition("prefetch", "dataProviders/dataProvider[@ref='dataProviders/main']/prefetch/cacheSize", "Caching.DefaultDataCacheSize"),
    }.ToMap(x => x.Name, x => x);

    [CanBeNull]
    [UsedImplicitly]
    public XmlElement Definition { get; }

    [NotNull]
    [UsedImplicitly]
    public string Name { get; }

    [NotNull]
    public IReadOnlyDictionary<string, LogicalDatabaseCache> Caches { get; }

    public LogicalDatabaseDefinition([NotNull] string name, [NotNull] IReadOnlyDictionary<string, LogicalDatabaseCache> caches, [CanBeNull] XmlElement definition = null)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      Name = name;
      Definition = definition;
      Caches = caches;
    }

    [NotNull]
    public static LogicalDatabaseDefinition Parse([NotNull] XmlElement definition, ISitecoreVersion version)
    {
      Assert.ArgumentNotNull(definition, nameof(definition));

      var name = definition.GetAttribute("id");
      Assert.ArgumentCondition(!string.IsNullOrWhiteSpace(name), nameof(definition), $"The id attribute is missing or whitespace.\r\nXml: {definition.OuterXml}");

      var caches = CacheDefinitions.Values
        .Select(x => new { x.Name, x.FallbackSizeSettingName, Definition = definition.SelectSingleNode(x.XPath) })
        .ToMap(x => x.Name, x => LogicalDatabaseCache.Parse(x.Name, x.FallbackSizeSettingName, x.Definition, version));

      return new LogicalDatabaseDefinition(name, caches, definition);
    }

    public override string ToString()
    {
      return Definition?.OuterXml ?? $"<database id=\"{Name}\"></database>";
    }

    private class CacheDefinition
    {
      public string Name { get; }

      public string XPath { get;  }

      public string FallbackSizeSettingName { get; }

      public CacheDefinition(string name, string xpath, string fallbackSizeSettingName)
      {
        Name = name;
        XPath = xpath;
        FallbackSizeSettingName = fallbackSizeSettingName;
      }
    }
  }
}