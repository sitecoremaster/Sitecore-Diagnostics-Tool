namespace Sitecore.DiagnosticsTool.Tests.UnitTests.WFFM
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Sitecore.DiagnosticsTool.Tests.WFFM;

  using Xunit;

  public class WffmSubmitFormTwiceTest : WffmSubmitFormTwice
  {
    [Fact]
    public void WffmSubmitFormTwicePassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 0, 2),
        })
        .AddResource(new LoggingContext())
        .Process(this)
        .Done();
    }

    [Fact]
    public void WffmSubmitFormTwiceError()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 0, 2),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Error,
              "Message: T-SQL ERROR 242, SEVERITY 16, STATE 3, PROCEDURE (null), LINE 823, MESSAGE: The conversion of a datetime data type to a smalldatetime data type resulted in an out-of-range value.")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();
    }
  }
}