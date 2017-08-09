namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public class ManagerDefinition
  {
    [UsedImplicitly]
    public ProviderDefinition DefaultProvider { get; }

    [UsedImplicitly]
    public string DefaultProviderName { get; }

    [UsedImplicitly]
    public XmlElement Definition { get; }

    [UsedImplicitly]
    public bool Enabled { get; }

    [UsedImplicitly]
    public string Name { get; }

    [NotNull]
    [UsedImplicitly]
    public IEnumerable<ProviderDefinition> Providers { get; }

    private ManagerDefinition([NotNull] string name, bool enabled, [NotNull] string defaultProviderName, [NotNull] ProviderDefinition[] providers, [NotNull] XmlElement definition)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(defaultProviderName, nameof(defaultProviderName));
      Assert.ArgumentNotNull(providers, nameof(providers));
      Assert.ArgumentNotNull(definition, nameof(definition));

      // last is used to support <clear />
      var defaultProvider = providers.LastOrDefault(x => x != null && x.Name == defaultProviderName);

      Definition = definition;
      Name = definition.Name;
      DefaultProviderName = defaultProviderName;
      DefaultProvider = defaultProvider;
      Providers = providers;
      Enabled = enabled;
    }

    [NotNull]
    public static ManagerDefinition Parse([NotNull] XmlElement definition)
    {
      Assert.ArgumentNotNull(definition, nameof(definition));

      var name = definition.Name;
      var enabled = definition.GetAttribute("enabled") != "false";
      var providers = definition.ChildNodes.OfType<XmlElement>().Select(x => ProviderDefinition.Parse(x)).ToArray();
      var defaultProviderName = definition.GetAttribute("defaultProvider");

      return new ManagerDefinition(name, enabled, defaultProviderName, providers, definition);
    }

    public override string ToString()
    {
      return Definition?.OuterXml ?? string.Format("<{0} enabled=\"{1}\" defaultProviderName=\"{2}\">\r\n  <providers>\r\n    {3}\r\n  </providers>\r\n</{0}>", Name, Enabled, DefaultProviderName, string.Join("\r\n    ", Providers.Select(x => x.ToString())));
    }
  }
}