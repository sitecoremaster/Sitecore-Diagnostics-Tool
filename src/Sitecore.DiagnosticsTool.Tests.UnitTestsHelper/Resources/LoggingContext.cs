namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;

  public class LoggingContext : ILoggingContext
  {
    [NotNull]
    private IEnumerable<ILogEntry> SitecoreLogEntries { get; }

    public LoggingContext([NotNull] params ILogEntry[] sitecoreLogEntries)
      : this((IEnumerable<ILogEntry>)sitecoreLogEntries)
    {
    }

    public LoggingContext([NotNull] IEnumerable<ILogEntry> sitecoreLogEntries)
    {
      Assert.ArgumentNotNull(sitecoreLogEntries, nameof(sitecoreLogEntries));

      SitecoreLogEntries = sitecoreLogEntries;
    }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries()
    {
      return GetSitecoreLogEntries(LogLevel.All, DateTime.MinValue, DateTime.MaxValue);
    }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries(LogLevel logLevel)
    {
      return GetSitecoreLogEntries(logLevel, DateTime.MinValue, DateTime.MaxValue);
    }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries(LogLevel logLevel, DateTime start, DateTime end)
    {
      return SitecoreLogEntries.Where(x => x != null && logLevel.HasFlag(x.Level) && x.Date >= start && x.Date <= end);
    }
  }
}