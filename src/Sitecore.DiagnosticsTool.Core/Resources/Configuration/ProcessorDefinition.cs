namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;

  public class ProcessorDefinition
  {
    [UsedImplicitly]
    public XmlElement Definition { get; }

    [UsedImplicitly]
    public bool Enabled { get; }

    [UsedImplicitly]
    public string Method { get; }

    [NotNull]
    [UsedImplicitly]
    public TypeRef Type { get; }

    public ProcessorDefinition([NotNull] TypeRef typeRef, [CanBeNull] string method = null, bool enabled = true, [CanBeNull] XmlElement definition = null)
    {
      Assert.ArgumentNotNull(typeRef, nameof(typeRef));

      Type = typeRef;
      Enabled = enabled;
      Method = method;
      Definition = definition;
    }

    [NotNull]
    public static ProcessorDefinition Parse([NotNull] XmlElement definition)
    {
      Assert.ArgumentNotNull(definition, nameof(definition));

      var type = definition.GetAttribute("type");
      var typeRef = TypeRef.Parse(type).IsNotNull("typeRef");
      var enabled = definition.GetAttribute("mode") != "off";
      var method = definition.GetAttribute("method");

      return new ProcessorDefinition(typeRef, method, enabled, definition);
    }

    public override bool Equals(object obj)
    {
      return ReferenceEquals(this, obj) || Equals(this, obj as ProcessorDefinition);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Method?.GetHashCode() ?? 0) * 397) ^ (Type != null ? Type.GetHashCode() : 0);
      }
    }

    public override string ToString()
    {
      return Definition?.OuterXml ?? $"<processor type=\"{Type}\"{(string.IsNullOrEmpty(Method) ? string.Empty : " method=\"" + Method + "\"")} mode=\"{(Enabled ? "on" : "off")}\" />";
    }

    public static bool Equals(ProcessorDefinition a, ProcessorDefinition b)
    {
      return ReferenceEquals(a, b) || !ReferenceEquals(a, null) && !ReferenceEquals(b, null) && string.Equals(a.Method, b.Method, StringComparison.Ordinal) && TypeRef.Equals(a.Type, b.Type);
    }

    public static bool operator ==(ProcessorDefinition a, object b)
    {
      return Equals(a, b as ProcessorDefinition);
    }

    public static bool operator !=(ProcessorDefinition a, object b)
    {
      return !Equals(a, b as ProcessorDefinition);
    }

    public static bool operator ==(object a, ProcessorDefinition b)
    {
      return Equals(a as ProcessorDefinition, b);
    }

    public static bool operator !=(object a, ProcessorDefinition b)
    {
      return !Equals(a as ProcessorDefinition, b);
    }

    public static bool operator ==(ProcessorDefinition a, ProcessorDefinition b)
    {
      return Equals(a, b);
    }

    public static bool operator !=(ProcessorDefinition a, ProcessorDefinition b)
    {
      return !Equals(a, b);
    }
  }
}