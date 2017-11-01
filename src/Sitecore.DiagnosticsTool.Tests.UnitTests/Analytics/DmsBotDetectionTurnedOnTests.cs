namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Analytics
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Analytics;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class DmsBotDetectionTurnedOnTests : DmsBotDetectionTurnedOn
  {
    [Fact]
    public void Test()
    {
      // NO DMS, NO DETECTION
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument().Create("/configuration/sitecore")
        })
        .Process(this)
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Analytics.Enabled' and @value='true']") // indicate DMS enabled
            .Add("/configuration/sitecore", "pipelines/startTracking") // indicate DMS enabled
            .Add("/configuration/sitecore", "analyticsExcludeRobots") // indicate exclude robots enabled
        })
        .Process(this)
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Analytics.Enabled' and @value='true']")
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ErrorMessage))
        .Done();
    }
  }
}