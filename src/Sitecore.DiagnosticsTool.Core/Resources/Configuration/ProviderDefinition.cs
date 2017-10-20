namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;

  public class ProviderDefinition
  {
    [UsedImplicitly]
    public XmlElement Definition { get; }

    [UsedImplicitly]
    public string Name { get; }

    [UsedImplicitly]
    public TypeRef Type { get; }

    private ProviderDefinition([NotNull] string name, [NotNull] TypeRef typeRef, XmlElement definition)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(typeRef, nameof(typeRef));

      Name = name;
      Type = typeRef;
      Definition = definition;
    }

    [NotNull]
    public static ProviderDefinition Parse([NotNull] XmlElement definition)
    {
      Assert.ArgumentNotNull(definition, nameof(definition));

      var type = definition.GetAttribute("type");
      var nameAttr = definition.Attributes["name"];
      var name = nameAttr?.Value;
      var typeRef = TypeRef.Parse(type);

      return new ProviderDefinition(name, typeRef, definition);
    }

    public override string ToString()
    {
      return Definition?.OuterXml ?? $"<add name=\"{Name}\" type=\"{Type}\" />";
    }
  }
}