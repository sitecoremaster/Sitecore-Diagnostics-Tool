namespace Sitecore.DiagnosticsTool.Core.Resources.Common
{
  public class DatabaseResourceNotAvailableException : ResourceNotAvailableException
  {
    public DatabaseResourceNotAvailableException(string details = null)
      : base(ResourceType.Database, details)
    {
    }
  }
}