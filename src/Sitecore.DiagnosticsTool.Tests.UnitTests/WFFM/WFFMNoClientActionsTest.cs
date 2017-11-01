namespace Sitecore.DiagnosticsTool.Tests.UnitTests.WFFM
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Sitecore.DiagnosticsTool.Tests.WFFM;

  using Xunit;

  public class WffmNoClientActionsTest : WffmNoClientActions
  {
    [Fact]
    public void WffmNoClientActionsTestPassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          ServerRoles = new[] {ServerRole.ContentDelivery},
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(new LoggingContext())
        .Process(this)
        .Done();
    }

    [Fact]
    public void WffmNoClientActionsTestError()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          ServerRoles = new[] {ServerRole.ContentDelivery},
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Warn,
              "form has no actions")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();
    }
  }
}