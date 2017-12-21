namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ContentSearchConfigurationsDuplicates : ILegacyTest
  {
    private const string ContentSearchXPath = "/configuration/sitecore/contentSearch";

    public string Name { get; } = "Duplication ContentSearch configuration nodes";

    public IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing };

    public bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data)
    {
      return data.SitecoreInfo.Configuration.SelectElements(ContentSearchXPath + "/*").Any();
    }

    public void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var duplicates = new Map<List<string>>();
      var resultConfig = data.SitecoreInfo.Configuration;
      var configurations = resultConfig.SelectElements(ContentSearchXPath + "/*").Select(x => x.Name).Distinct().ToArray();
      foreach (var configuration in configurations)
      {
        var configurationElements = resultConfig.SelectElements($"{ContentSearchXPath}/{configuration}");
        if (configurationElements.Length > 1)
        {
          var list = duplicates.GetOrAdd(configuration, new List<string>());
          list.AddRange(configurationElements.Select(x => x.ToString(XmlPrintMode.HeaderOnly)));
        }
      }

      if (duplicates.Any())
      {
        var shortMessage = "There are duplicates in ContentSearch configuration nodes that may lead to unpredictable behavior.";
        var detailed = new DetailedMessage(
          new Text("The following ContentSearch configurations are duplicated:"),
          new BulletedList(duplicates.Select(x => new Container(
            new Text(x.Key),
            new BulletedList(x.Value.Select(z => new Code(z)))
          ))),
          new Text("To resolve, rewise the configuration files and check the difference between defintions."));

        output.Warning(shortMessage, null, detailed);
      }
    }
  }
}