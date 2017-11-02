namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class CounterCategoryDoesNotExist : Test
  {
    protected const string CountersNotInstalledMessage = "Counters do not seem to be installed on the server.";

    [NotNull]
    protected Uri Link => new Uri("http://sdn.sitecore.net/Scrapbook/Working%20with%20Sitecore%20Performance%20Counters.aspx");

    public override string Name { get; } = "Sitecore counters are installed";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var warnLogEntries = data.Logs.GetSitecoreLogEntries(LogLevel.Warn);
      if (warnLogEntries.Any(entry => entry.Message.StartsWith("Counter category '") && entry.Message.Contains("' does not exist on this server. Using temporary public counter for '")))
      {
        output.Warning(CountersNotInstalledMessage, Link);
      }
    }
  }
}