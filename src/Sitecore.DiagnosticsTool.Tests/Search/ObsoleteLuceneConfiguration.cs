namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ObsoleteLuceneConfiguration : Test
  {
    private const string ContentSearchXPath = "/configuration/sitecore/contentSearch";

    [NotNull]
    private TypeRef ObsoleteType { get; } = TypeRef.Parse("Sitecore.ContentSearch.LuceneProvider.LuceneSearchConfiguration, Sitecore.ContentSearch.LuceneProvider");

    public override string Name { get; } = "Obsolete lucene configuration";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing };

    public override bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, IInstanceResourceContext data)
    {
      return data.SitecoreInfo.Configuration.SelectElements($"{ContentSearchXPath}/*").Any();
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      var obsoleteConfigurations = GetObsoleteConfigurations(data).ToArray();

      foreach (var configuration in obsoleteConfigurations)
      {
        var configurationName = configuration.Name;
        var configurationFiles = configuration.Files;

        foreach (var configurationFile in data.SitecoreInfo.ConfigurationFiles.Values)
        {
          var configurationElements = configurationFile.Configuration.SelectElements($"{ContentSearchXPath}/{configurationName}");
          if (configurationElements.Any(x => TypeRef.Parse(x.GetAttribute("type")) == configuration.Type))
          {
            configurationFiles.Add(configurationFile.FilePath.Substring(configurationFile.FilePath.IndexOf("App_Config")));
          }
        }
      }

      if (obsoleteConfigurations.Any(x => x.Files.Any()))
      {
        var defaultType = TypeRef.Parse(data.SitecoreInfo.SitecoreDefaults.Configuration.SelectSingleElement($"{ContentSearchXPath}/configuration").GetAttribute("type"));
        Assert.IsNotNull(defaultType);

        var shortMessage = "ContentSearch configuration uses obsolete LuceneSearchConfiguration that may lead to unpredictable behavior.";
        var detailed = new DetailedMessage(
          new Text("The following ContentSearch configurations are obsolete:"),
          new BulletedList(obsoleteConfigurations.Select(x =>
            new Container(
              new Code(x.Definition.ToString(XmlPrintMode.HeaderOnly)),
              new BulletedList(x.Files)))),
          new Text("To resolve, change configuration type either to "),
          new Code(defaultType.ToString()),
          new Text(", or to a custom class that inherits from it."));

        output.Warning(shortMessage, null, detailed);
      }
    }

    [ItemNotNull]
    private IEnumerable<ObsoleteConfiguration> GetObsoleteConfigurations(IInstanceResourceContext data)
    {
      var configurationElements = data.SitecoreInfo.Configuration.SelectElements($"{ContentSearchXPath}/*");
      foreach (var configurationElement in configurationElements)
      {
        var typeName = configurationElement.GetAttribute("type");
        var type = TypeRef.Parse(typeName);
        if (type == ObsoleteType)
        {
          yield return new ObsoleteConfiguration(configurationElement.Name, type, configurationElement);
        }
      }
    }

    private class ObsoleteConfiguration
    {
      [NotNull]
      public string Name { get; }

      [NotNull]
      public TypeRef Type { get; }

      [NotNull]
      public XmlElement Definition { get; }

      [NotNull]
      public IList<string> Files { get; } = new List<string>();

      public ObsoleteConfiguration([NotNull] string name, [NotNull] TypeRef type, [NotNull] XmlElement definition)
      {
        Name = name;
        Definition = definition;
        Type = type;
      }
    }
  }
}
