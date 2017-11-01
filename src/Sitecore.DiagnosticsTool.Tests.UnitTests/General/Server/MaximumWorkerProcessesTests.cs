namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Server
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Server;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class MaximumWorkerProcessesTests : MaximumWorkerProcesses
  {
    [Fact]
    public void Test()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(new WebServer
        {
          CurrentSite = new Site
          {
            ApplicationPool = new ApplicationPool
            {
              MaxWorkerProcesses = 1
            }
          }
        })
        .Process(this)
        .Done();

      var pool = new ApplicationPool
      {
        MaxWorkerProcesses = 2
      };

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2, 161221),
        })
        .AddResource(new WebServer
        {
          CurrentSite = new Site
          {
            ApplicationPool = pool
          }
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetErrorMessage(pool)))
        .Done();
    }
  }
}