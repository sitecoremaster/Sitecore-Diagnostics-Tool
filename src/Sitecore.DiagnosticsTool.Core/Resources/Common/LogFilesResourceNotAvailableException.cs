namespace Sitecore.DiagnosticsTool.Core.Resources.Common
{
  public class LogFilesResourceNotAvailableException : ResourceNotAvailableException
  {
    public LogFilesResourceNotAvailableException(string details = null)
      : base(ResourceType.LogFiles, details)
    {
    }
  }
}