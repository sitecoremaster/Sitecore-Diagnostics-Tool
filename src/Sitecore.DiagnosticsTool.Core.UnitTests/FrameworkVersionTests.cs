namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Xunit;

  public class FrameworkVersionTests
  {
    [Fact]
    public void TestFrameworkVersion()
    {
      Assert.Equal(false, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v2x));
      Assert.Equal(false, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v20));
      Assert.Equal(false, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v35));
      Assert.Equal(false, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v4x));
      Assert.Equal(false, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v40));
      Assert.Equal(true, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v45x));
      Assert.Equal(true, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v450));
      Assert.Equal(true, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v451));
      Assert.Equal(true, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v452));
      Assert.Equal(true, FrameworkVersion.v45x.HasFlag(FrameworkVersion.v460));
    }
  }
}