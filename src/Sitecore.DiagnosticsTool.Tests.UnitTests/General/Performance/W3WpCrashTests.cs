namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System;
  using System.Linq;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Performance;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class W3WpCrashTests : W3WpCrash
  {
    /// <summary>
    ///   Passed, restart was within the acceptable timeout
    /// </summary>
    [Fact]
    public void W3WpCrashTestRestart()
    {
      var shutdown = DateTime.Now;
      var start = shutdown.Add(ShutdownTimeout);

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(
          LogHelperEx.Parse(shutdown, LogLevel.Warn, ShutdownMessage),
          LogHelperEx.Parse(start, LogLevel.Info, StartMessage)))
        .Process(this)
        .Done();
    }

    /// <summary>
    ///   Passed, restart was out of the acceptable timeout, but within allowed acceptable log entries count
    /// </summary>
    [Fact]
    public void W3WpCrashTestLongAgoRestart()
    {
      const int Count = MaxEntriesCount - 2;
      var shutdown = DateTime.Now;
      var extra = shutdown.AddSeconds(1);
      var start = extra.Add(ShutdownTimeout);

      // insert shutdown message marker
      var entries = new[] { LogHelperEx.Parse(shutdown, LogLevel.Warn, ShutdownMessage) }.ToList();

      // insert 100 (MaxEntriesCount) "Job started" messages
      entries.AddRange(Enumerable.Repeat(LogHelperEx.Parse(extra, LogLevel.Info, "Job started"), Count));

      // insert start message marker
      entries.Add(LogHelperEx.Parse(start, LogLevel.Info, StartMessage));

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(entries.ToArray()))
        .Process(this)
        .Done();
    }

    /// <summary>
    ///   Passed, cold start (no entries before start message)
    /// </summary>
    [Fact]
    public void W3WpCrashTestColdStart()
    {
      var start = DateTime.Now;

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(
          LogHelperEx.Parse(start, LogLevel.Info, "HttpModule is being initialized"),
          LogHelperEx.Parse(start, LogLevel.Info, string.Empty),
          LogHelperEx.Parse(start, LogLevel.Info, "**********************************************************************"),
          LogHelperEx.Parse(start, LogLevel.Info, "**********************************************************************"),
          LogHelperEx.Parse(start, LogLevel.Info, StartMessage)))
        .Process(this)
        .Done();
    }

    /// <summary>
    ///   Warning, crash happened within acceptable timeout and entries count
    /// </summary>
    [Fact]
    public void W3WpCrashTestCrash()
    {
      var crash = DateTime.Now;
      var start = crash.Add(ShutdownTimeout);

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(
          LogHelperEx.Parse(crash, LogLevel.Info, "Job Started"),
          LogHelperEx.Parse(crash, LogLevel.Info, "Job Started"),
          LogHelperEx.Parse(crash, LogLevel.Info, "Job Started"),
          LogHelperEx.Parse(start, LogLevel.Info, "HttpModule is being initialized"),
          LogHelperEx.Parse(start, LogLevel.Info, string.Empty),
          LogHelperEx.Parse(start, LogLevel.Info, "**********************************************************************"),
          LogHelperEx.Parse(start, LogLevel.Info, "**********************************************************************"),
          LogHelperEx.Parse(start, LogLevel.Info, StartMessage)))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetMessage(start)))
        .Done();
    }

    /// <summary>
    ///   Passed, crash happened out of the acceptable timeout and log entries count
    /// </summary>
    [Fact]
    public void W3WpCrashTestTooLongRestart()
    {
      const int Count = MaxEntriesCount;
      var shutdown = DateTime.Now;
      var extra = shutdown.AddSeconds(1);
      var start = extra.Add(ShutdownTimeout);

      // insert shutdown message marker
      var entries = new[] { LogHelperEx.Parse(shutdown, LogLevel.Warn, ShutdownMessage) }.ToList();

      // insert 100 (MaxEntriesCount) "Job started" messages
      entries.AddRange(Enumerable.Repeat(LogHelperEx.Parse(extra, LogLevel.Info, "Job started"), Count));

      // insert start message marker
      entries.Add(LogHelperEx.Parse(start, LogLevel.Info, StartMessage));

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(entries.ToArray()))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetMessage(start)))
        .Done();
    }
  }
}