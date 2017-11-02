namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Solution
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Solution;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class ConsistentConfigurationTest : ConsistentConfiguration
  {
    [Fact]
    public void ConsistentConfiguration1()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void ConsistentConfiguration2()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/ONE.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void InconsistentConfiguration1()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<ONE />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ShortMessage))
        .Done();
    }

    [Fact]
    public void InconsistentConfiguration2()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/one.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            Version = new SitecoreVersion(9, 0, 0, 171002),
            IncludeFiles = new[] { new ConfigurationFile("/App_Config/Include/two.config", "<one />") }.ToMap(x => x.FilePath.ToLowerInvariant(), x => x)
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ShortMessage))
        .Done();
    }
  }
}