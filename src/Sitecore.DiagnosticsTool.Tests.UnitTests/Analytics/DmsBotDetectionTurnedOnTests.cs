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
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create("/configuration/sitecore")
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Test1()
    {
      new SolutionUnitTestContext()
          
          .AddInstance(new SitecoreInstance
          {
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument()
              .Create("/configuration/sitecore/settings/setting[@name='Xdb.Enabled' and @value='true']") // indicate xDB enabled
              .Add("/configuration/sitecore", "pipelines/createTracker") // indicate xDB enabled
              .Add("/configuration/sitecore", "analyticsExcludeRobots") // indicate exclude robots enabled
          })
          .Process(this)
          .Done();

    }

    [Fact]
    public void Test2()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Xdb.Enabled' and @value='true']")
            .Add("/configuration/sitecore", "pipelines/createTracker") // indicate xDB enabled
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ErrorMessage))
        .Done();
    }
  }
}