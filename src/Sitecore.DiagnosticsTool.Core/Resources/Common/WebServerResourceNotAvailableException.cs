namespace Sitecore.DiagnosticsTool.Core.Resources.Common
{
  public class WebServerResourceNotAvailableException : ResourceNotAvailableException
  {
    public WebServerResourceNotAvailableException(string details = null)
      : base(ResourceType.WebServer, details)
    {
    }
  }
}