namespace Sitecore.DiagnosticsTool.Core.UnitTests
{
  using FluentAssertions;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Xunit;

  public class ResourceNotAvailableExceptionTests
  {
    [Fact]
    public void ResourceNotAvailableExceptionTest()
    {
      var exception = new WebServerResourceNotAvailableException();
      exception.Message.Should().NotBeNullOrEmpty();
      exception.ResourceType.Should().Be(ResourceType.WebServer);
    }
  }
}