namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class PublishLogging : Test
  {
    protected const string MessageTraceToLogEnabled = "TraceToLog option is enabled for publishing which may affect performance. To turn it off, set <traceToLog>false</traceToLog> setting in web.config.";

    public override string Name { get; } = "Publish Trace logging is disabled";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Production, Category.Performance };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var config = data.SitecoreInfo.Configuration;
      var traceToLogElement = config.SelectSingleNode("/configuration/sitecore/pipelines/publishItem/processor[@type='Sitecore.Publishing.Pipelines.PublishItem.UpdateStatistics, Sitecore.Kernel']/traceToLog");
      if (traceToLogElement != null && traceToLogElement.InnerText.Equals("true", StringComparison.OrdinalIgnoreCase))
      {
        output.Error(MessageTraceToLogEnabled);
      }
    }
  }
}