namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.UnitTests.Resources
{
  using System;
  using System.Linq;

  using FluentAssertions;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  [TestClass]
  public class LoggingContextTests
  {
    [TestMethod]
    public void GetSitecoreLogEntriesTest()
    {
      var now = DateTime.Now;
      var entries = new[]
      {
        // 1
        LogHelperEx.Parse(now.AddSeconds(1), LogLevel.Debug, "1"),

        // 2
        LogHelperEx.Parse(now.AddSeconds(2), LogLevel.Info, "2"),
        LogHelperEx.Parse(now.AddSeconds(3), LogLevel.Info, "3"),

        // 3
        LogHelperEx.Parse(now.AddSeconds(4), LogLevel.Warn, "4"),
        LogHelperEx.Parse(now.AddSeconds(5), LogLevel.Warn, "5"),
        LogHelperEx.Parse(now.AddSeconds(6), LogLevel.Warn, "6"),

        // 4
        LogHelperEx.Parse(now.AddSeconds(7), LogLevel.Error, "7"),
        LogHelperEx.Parse(now.AddSeconds(8), LogLevel.Error, "8"),
        LogHelperEx.Parse(now.AddSeconds(9), LogLevel.Error, "9"),
        LogHelperEx.Parse(now.AddSeconds(10), LogLevel.Error, "10"),

        // 5
        LogHelperEx.Parse(now.AddSeconds(11), LogLevel.Fatal, "11"),
        LogHelperEx.Parse(now.AddSeconds(12), LogLevel.Fatal, "12"),
        LogHelperEx.Parse(now.AddSeconds(13), LogLevel.Fatal, "13"),
        LogHelperEx.Parse(now.AddSeconds(14), LogLevel.Fatal, "14"),
        LogHelperEx.Parse(now.AddSeconds(15), LogLevel.Fatal, "15"),

        // 6
        LogHelperEx.Parse(now.AddSeconds(16), LogLevel.Critical, "16"),
        LogHelperEx.Parse(now.AddSeconds(17), LogLevel.Critical, "17"),
        LogHelperEx.Parse(now.AddSeconds(18), LogLevel.Critical, "18"),
        LogHelperEx.Parse(now.AddSeconds(19), LogLevel.Critical, "19"),
        LogHelperEx.Parse(now.AddSeconds(20), LogLevel.Critical, "20"),
        LogHelperEx.Parse(now.AddSeconds(21), LogLevel.Critical, "21")
      };

      var context = new LoggingContext(entries);
      context.GetSitecoreLogEntries(LogLevel.All).Count().Should().Be(21);
      context.GetSitecoreLogEntries(LogLevel.Debug).Count().Should().Be(1);
      context.GetSitecoreLogEntries(LogLevel.Info).Count().Should().Be(2);
      context.GetSitecoreLogEntries(LogLevel.Warn).Count().Should().Be(3);
      context.GetSitecoreLogEntries(LogLevel.Error).Count().Should().Be(4);
      context.GetSitecoreLogEntries(LogLevel.Fatal).Count().Should().Be(5);
      context.GetSitecoreLogEntries(LogLevel.Critical).Count().Should().Be(6);
      context.GetSitecoreLogEntries(LogLevel.Fatal | LogLevel.Critical).Count().Should().Be(5 + 6);
    }
  }
}