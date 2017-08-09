namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Health
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Health;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Xunit;

  public class ErrorWhenPublishingTheCheckInItemTests : ErrorWhenPublishingTheCheckInItem
  {
    [Fact]
    public void TestPassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(new LoggingContext())
        .Process(this)
        .Done();
    }

    [Fact]
    public void TestError()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(new LoggingContext(LogHelperEx.Parse(LogLevel.Error, @"Cannot find dictionary domain for the dictionaty entry '/sitecore/system/Settings/Workflow/Check in'('{49D654D8-19C1-4DFC-BE0F-7A7D2314340F}')")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, Message, Link))
        .Done();
    }
  }
}