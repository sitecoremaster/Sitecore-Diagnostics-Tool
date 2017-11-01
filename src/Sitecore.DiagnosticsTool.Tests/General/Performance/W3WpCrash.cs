namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class W3WpCrash : Test
  {
    protected const string StartMessage = "Sitecore started";

    protected const string ShutdownMessage = "Sitecore shutting down";

    protected const int MaxEntriesCount = 100;

    protected static readonly TimeSpan ShutdownTimeout = new TimeSpan(0, 0, 2, 0, 0);

    public override string Name { get; } = "Hard application restarts";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      var moments = new List<DateTime>();
      var startMessages = data.Logs.GetSitecoreLogEntries(LogLevel.Info).Where(x => x.Message.StartsWith(StartMessage, StringComparison.OrdinalIgnoreCase));
      foreach (var entry in startMessages)
      {
        var moment = entry.Date;
        var entries = data.Logs.GetSitecoreLogEntries(LogLevel.All, moment - ShutdownTimeout, moment);
        var entriesToSearch = GetEntriesToSearch(entries, entry).ToArray();

        // if cold start
        if (entriesToSearch.Length <= 5)
        {
          continue;
        }

        if (entriesToSearch.Any(x => x.Level == LogLevel.Warn && x.Message.StartsWith(ShutdownMessage)))
        {
          continue;
        }

        var left = MaxEntriesCount - entriesToSearch.Length;
        if (left > 0)
        {
          var extraEntries = data.Logs.GetSitecoreLogEntries(LogLevel.All, DateTime.MinValue, moment - ShutdownTimeout).ToArray();

          if (extraEntries.Length > left)
          {
            extraEntries = extraEntries.Take(left).ToArray();
          }

          if (extraEntries.Any(x => x.Level == LogLevel.Warn && x.Message.StartsWith(ShutdownMessage)))
          {
            continue;
          }
        }

        moments.Add(moment);
      }

      if (moments.Count > 0)
      {
        output.Warning(GetMessage(moments));
      }
    }

    private static IEnumerable<ILogEntry> GetEntriesToSearch(IEnumerable<ILogEntry> entries, ILogEntry entry)
    {
      foreach (var logEntry in entries)
      {
        if (logEntry == entry)
        {
          yield break;
        }

        yield return logEntry;
      }
    }

    [NotNull]
    protected string GetMessage(DateTime moment)
    {
      return $"Potential hard application restart has been detected around {moment.ToUniversalTime().ToString("dd-MMM-yyyy, HH:mm UTC")}.";
    }

    [NotNull]
    protected string GetMessage(List<DateTime> moments)
    {
      return moments.Count == 1 ? GetMessage(moments.First()) : $"Potential {moments.Count} hard application restarts have been detected. First occurrence around {moments.First().ToUniversalTime().ToString("dd-MMM-yyyy, HH:mm UTC")}, last occurrence around {moments.Last().ToUniversalTime().ToString("dd-MMM-yyyy, HH:mm UTC")}.";
    }
  }
}