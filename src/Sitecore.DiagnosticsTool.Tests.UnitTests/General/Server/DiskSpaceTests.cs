namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Server
{
  using System.IO;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Server;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Xunit;

  public class DiskSpaceTests : DiskSpace
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
            WebRoot = new DirectoryInfo("C:\\")
          }
        })
        .AddResource(new FileSystemContext
        {
          Drives = new DriveContext(new Drive("C:\\")
          {
            AvailableFreeSpace = Size.FromGb(Recommended)
          })
        })
        .Process(this)
        .Done();

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
            WebRoot = new DirectoryInfo("C:\\")
          }
        })
        .AddResource(new FileSystemContext
        {
          Drives = new DriveContext(new Drive("C:\\")
          {
            AvailableFreeSpace = Size.FromGb(0.5)
          })
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetErrorMessage(Size.FromGb(0.5), "C:\\")))
        .Done();

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
            WebRoot = new DirectoryInfo("C:\\")
          }
        })
        .AddResource(new FileSystemContext
        {
          Drives = new DriveContext(new Drive("C:\\")
          {
            AvailableFreeSpace = Size.FromGb(Recommended - 1)
          })
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetWarningMessage(Size.FromGb(Recommended - 1), "C:\\")))
        .Done();
    }
  }
}