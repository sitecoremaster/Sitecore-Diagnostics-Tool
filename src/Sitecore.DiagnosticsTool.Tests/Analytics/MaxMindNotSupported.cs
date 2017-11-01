namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, log-based)
  [UsedImplicitly]
  public class MaxMindNotSupported : Test
  {
    protected const string Message = "The Maxmind-based GeoIP Lookup Service is discontinued since August 31, 2015";

    protected const string Exception = "Failed to perform MaxMind lookup";

    protected Uri Url { get; } = new Uri(@"https://doc.sitecore.net/sitecore_experience_platform/ip_geolocation/setting_up_sitecore_ip_geolocation#_Migrating_from_MaxMind");

    public override string Name { get; } = "MaxMind GeoIP Lookup Service discontinuation";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.Error);
      foreach (var log in logs)
      {
        var ex = log.RawText;
        if (ex.Contains(Exception))
        {
          output.Error(Message, url: Url);
          return;
        }
      }
    }
  }
}