namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  public class ContentSearchIndexes : Dictionary<string, IContentSearchIndex>
  {
    public ContentSearchIndexes([NotNull] XmlDocument configuration)
      : base(GetDictionary(configuration))
    {
    }

    private static IDictionary<string, IContentSearchIndex> GetDictionary(XmlDocument configuration)
    {
      return configuration.SelectElements($@"/configuration/sitecore/contentSearch/configuration/indexes/*")
        .Select(x => TryParseContentSearchIndex(x))
        .ToDictionary(x => x.Id, x => x);
    }

    private static IContentSearchIndex TryParseContentSearchIndex([NotNull] XmlElement indexConfiguration)
    {
      try
      {
        return new ContentSearchIndex(indexConfiguration);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot parse ContentSearch index: {indexConfiguration.OuterXml}");

        return null;
      }
    }
  }
}