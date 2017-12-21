namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Server
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Server;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class CounterCategoryDoesNotExistTests : CounterCategoryDoesNotExist
  {
    [Fact]
    public void Test()
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

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
        })
        .AddResource(new LoggingContext(LogHelperEx.Parse(LogLevel.Warn, "Counter category 'xxx' does not exist on this server. Using temporary public counter for '")))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, CountersNotInstalledMessage, Link))
        .Done();
    }
  }
}