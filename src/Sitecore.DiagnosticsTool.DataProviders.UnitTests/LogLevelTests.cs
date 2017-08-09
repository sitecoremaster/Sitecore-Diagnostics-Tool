namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;

  [TestClass]
  public class LogLevelTests
  {
    [TestMethod]
    public void AllHasFlagTest()
    {
      const LogLevel Level = LogLevel.All;

      Level.HasFlag(LogLevel.All).Should().BeTrue();
      Level.HasFlag(LogLevel.Debug).Should().BeTrue();
      Level.HasFlag(LogLevel.Info).Should().BeTrue();
      Level.HasFlag(LogLevel.Warn).Should().BeTrue();
      Level.HasFlag(LogLevel.Error).Should().BeTrue();
      Level.HasFlag(LogLevel.Fatal).Should().BeTrue();
      Level.HasFlag(LogLevel.Critical).Should().BeTrue();
    }

    [TestMethod]
    public void DebugHasFlagTest()
    {
      const LogLevel Level = LogLevel.Debug;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeTrue();
      Level.HasFlag(LogLevel.Info).Should().BeFalse();
      Level.HasFlag(LogLevel.Warn).Should().BeFalse();
      Level.HasFlag(LogLevel.Error).Should().BeFalse();
      Level.HasFlag(LogLevel.Fatal).Should().BeFalse();
      Level.HasFlag(LogLevel.Critical).Should().BeFalse();
    }

    [TestMethod]
    public void InfoHasFlagTest()
    {
      const LogLevel Level = LogLevel.Info;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeFalse();
      Level.HasFlag(LogLevel.Info).Should().BeTrue();
      Level.HasFlag(LogLevel.Warn).Should().BeFalse();
      Level.HasFlag(LogLevel.Error).Should().BeFalse();
      Level.HasFlag(LogLevel.Fatal).Should().BeFalse();
      Level.HasFlag(LogLevel.Critical).Should().BeFalse();
    }

    [TestMethod]
    public void WarnHasFlagTest()
    {
      const LogLevel Level = LogLevel.Warn;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeFalse();
      Level.HasFlag(LogLevel.Info).Should().BeFalse();
      Level.HasFlag(LogLevel.Warn).Should().BeTrue();
      Level.HasFlag(LogLevel.Error).Should().BeFalse();
      Level.HasFlag(LogLevel.Fatal).Should().BeFalse();
      Level.HasFlag(LogLevel.Critical).Should().BeFalse();
    }

    [TestMethod]
    public void ErrorHasFlagTest()
    {
      const LogLevel Level = LogLevel.Error;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeFalse();
      Level.HasFlag(LogLevel.Info).Should().BeFalse();
      Level.HasFlag(LogLevel.Warn).Should().BeFalse();
      Level.HasFlag(LogLevel.Error).Should().BeTrue();
      Level.HasFlag(LogLevel.Fatal).Should().BeFalse();
      Level.HasFlag(LogLevel.Critical).Should().BeFalse();
    }

    [TestMethod]
    public void FatalHasFlagTest()
    {
      const LogLevel Level = LogLevel.Fatal;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeFalse();
      Level.HasFlag(LogLevel.Info).Should().BeFalse();
      Level.HasFlag(LogLevel.Warn).Should().BeFalse();
      Level.HasFlag(LogLevel.Error).Should().BeFalse();
      Level.HasFlag(LogLevel.Fatal).Should().BeTrue();
      Level.HasFlag(LogLevel.Critical).Should().BeFalse();
    }

    [TestMethod]
    public void CriticalHasFlagTest()
    {
      const LogLevel Level = LogLevel.Critical;

      Level.HasFlag(LogLevel.All).Should().BeFalse();
      Level.HasFlag(LogLevel.Debug).Should().BeFalse();
      Level.HasFlag(LogLevel.Info).Should().BeFalse();
      Level.HasFlag(LogLevel.Warn).Should().BeFalse();
      Level.HasFlag(LogLevel.Error).Should().BeFalse();
      Level.HasFlag(LogLevel.Fatal).Should().BeFalse();
      Level.HasFlag(LogLevel.Critical).Should().BeTrue();
    }
  }
}