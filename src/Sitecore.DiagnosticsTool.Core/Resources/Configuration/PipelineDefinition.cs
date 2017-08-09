namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public class PipelineDefinition
  {
    [CanBeNull]
    [UsedImplicitly]
    public XmlElement Definition { get; }

    /// <summary>
    ///   The name of pipeline with group prefix (for example, pipelines/itemProvider/addFromTemplate)
    /// </summary>
    [NotNull]
    [UsedImplicitly]
    public string Name { get; }

    [NotNull]
    [UsedImplicitly]
    public IReadOnlyList<ProcessorDefinition> Processors { get; }

    public PipelineDefinition([NotNull] string name, [NotNull] IEnumerable<ProcessorDefinition> processors, [CanBeNull] XmlElement definition = null)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(processors, nameof(processors));

      Name = name;
      Processors = processors.ToArray();
      Definition = definition;
    }

    [NotNull]
    public static PipelineDefinition Parse([NotNull] XmlElement definition, [CanBeNull] string prefix = null)
    {
      Assert.ArgumentNotNull(definition, nameof(definition));

      var name = $"{prefix}/{definition.Name}".TrimStart('/');
      var processors = definition.ChildNodes.OfType<XmlElement>().Select(x => ProcessorDefinition.Parse(x));

      return new PipelineDefinition(name, processors, definition);
    }

    public override string ToString()
    {
      return Definition?.OuterXml ?? string.Format("<{0}>\r\n  {1}\r\n</{0}>", Name, string.Join("\r\n  ", Processors.Select(x => x.ToString())));
    }
  }
}