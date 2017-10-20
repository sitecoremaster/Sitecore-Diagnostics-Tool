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

  public class WffmObsoleteCommandsTest : WffmObsoleteCommands
  {
    [Fact]
    public void WffmObsoleteCommandsTestPassed()
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
    public void WffmObsoleteCommandsTestError()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Error,
              "Could not instantiate \"Sitecore.Forms.Core.Commands.View.Refresh,Sitecore.Forms.Core\" command object.")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Error,
              "Could not instantiate \"Sitecore.Forms.Core.Commands.Fields.SelectFields,Sitecore.Forms.Core\" command object.")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(
          new LoggingContext(
            LogHelperEx.Parse(
              LogLevel.Error,
              "Could not instantiate \"Sitecore.Form.Core.Commands.OpenSession,Sitecore.Forms.Core\" command object.")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, $"The known issue #{KbNumber} can be potentially applicable to the solution", Link))
        .Done();
    }
  }
}