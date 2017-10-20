namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;

  public class LogicalDatabaseCache
  {
    [NotNull]
    public string Name { get; }

    [CanBeNull]
    public CacheSize Size { get; }

    [CanBeNull]
    public XmlNode Definition { get; }

    [NotNull]
    public string FallbackSettingName { get; }

    public LogicalDatabaseCache([NotNull] string name, [NotNull] string fallbackSettingName, [CanBeNull] CacheSize size, [CanBeNull] XmlNode definition = null)
    {
      Name = name;
      Size = size;
      FallbackSettingName = fallbackSettingName;
      Definition = definition;
    }

    [NotNull]
    public static LogicalDatabaseCache Parse([NotNull] string name, [NotNull] string fallbackSettingName, [CanBeNull] XmlNode definition, ISitecoreVersion version)
    {
      if (definition == null)
      {
        return new LogicalDatabaseCache(name, fallbackSettingName, null);
      }

      var size = CacheSize.Parse(definition.InnerText, version);
      var cache = new LogicalDatabaseCache(name, fallbackSettingName, size, definition);

      return cache;
    }
  }
}