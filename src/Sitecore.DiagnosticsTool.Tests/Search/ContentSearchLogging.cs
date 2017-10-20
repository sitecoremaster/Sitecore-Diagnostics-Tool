namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class ContentSearchLogging : Test
  {
    protected const string MessageVerboseLoggingEnabled = "The ContentSearch.VerboseLogging is enabled which may affect performance. Read more in Sitecore Search and Indexing Guide, section Verbose Logger.";

    protected const string XPath = @"/configuration/sitecore/settings/setting";

    protected const string AttributeName = @"ContentSearch.VerboseLogging";

    public override string Name { get; } = "Content Search Verbose Logging is disabled";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing, Category.Production };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var config = data.SitecoreInfo.Configuration;
      var verboseLoggingElement = config.SelectSingleNode(XPath + "[@name='" + AttributeName + "']") as XmlElement;
      if (verboseLoggingElement == null)
      {
        return;
      }

      var verboseLoggingValue = verboseLoggingElement.GetAttribute("value");
      if (verboseLoggingValue.Equals("true", StringComparison.OrdinalIgnoreCase))
      {
        output.Warning(MessageVerboseLoggingEnabled);
      }
    }
  }
}