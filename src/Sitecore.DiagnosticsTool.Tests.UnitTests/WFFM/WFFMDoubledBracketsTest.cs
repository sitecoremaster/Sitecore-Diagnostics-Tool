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

  public class WffmDoubledBracketsTest : WffmDoubledBrackets
  {
    [Fact]
    public void WffmDoubledBracketsPassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext())
        .Process(this)
        .Done();
    }

    [Fact]
    public void WffmDoubledBracketsError()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Warn,
              "The specified string is not in the form required for an e-mail address.")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();
    }
  }
}