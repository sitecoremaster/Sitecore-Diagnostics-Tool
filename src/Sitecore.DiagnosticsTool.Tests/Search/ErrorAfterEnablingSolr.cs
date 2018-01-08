namespace Sitecore.DiagnosticsTool.Tests.Search
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class ErrorAfterEnablingSolr : KbTest
  {
    public override string KbNumber => "683462";

    public override string KbName { get; } = "Error after enabling Solr in Sitecore 8";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var ex = log.Exception;
        if (ex == null)
        {
          continue;
        }

        if (ex.Message.Contains("Could not find property 'typeMatches' on object of type: Sitecore.ContentSearch.SolrProvider.SolrFieldMap"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}