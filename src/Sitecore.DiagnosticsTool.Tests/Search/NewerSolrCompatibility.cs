namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: TODO (2017-06-13)
  [UsedImplicitly]
  public class NewerSolrCompatibility : KbTest
  {
    public override string KbName { get; } = "Sitecore compatibility with Solr 4.8 and later";

    public override string KbNumber => "227897";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.SearchIndexing};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 7;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(output, nameof(output));

      var solrServiceBaseAddress = data.SitecoreInfo.GetSetting("ContentSearch.Solr.ServiceBaseAddress");
      if (string.IsNullOrWhiteSpace(solrServiceBaseAddress))
      {
        output.Debug("Solr is not being used or service base address is not set.");

        return;
      }

      var uriString = solrServiceBaseAddress + "/admin/info/system";
      var webRequest = data.WebServer.CurrentSite.CreateWebRequest(new Uri(uriString));
      var document = new XmlDocument();

      try
      {
        using (var response = webRequest.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            if (stream == null || stream == Stream.Null)
            {
              output.CannotRun("Solr server is not up or configured incorrectly.");

              return;
            }

            document.Load(stream);
          }
        }
      }
      catch (WebException e)
      {
        output.CannotRun("Solr service is not reachable. Launch Solr to run the test successfully");

        output.Debug(e, "Error during request to " + uriString);

        return;
      }

      var versionNode = document.SelectSingleNode(@"//*[@name='solr-spec-version']");
      if (versionNode == null)
      {
        output.CannotRun("Solr version cannot be identified.");

        output.Debug("Response xml: " + document.OuterXml);

        return;
      }

      var versionNodeValue = versionNode.InnerText;

      var version = Version.Parse(versionNodeValue);
      if (version >= Version.Parse("4.8"))
      {
        output.Warning("Solr 4.8+ is used. Issues with generating schema are possible.", url: Link);
      }
    }
  }
}