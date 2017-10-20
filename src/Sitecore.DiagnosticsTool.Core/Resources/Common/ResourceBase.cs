namespace Sitecore.DiagnosticsTool.Core.Resources.Common
{
  using JetBrains.Annotations;

  public abstract class ResourceBase
  {
    protected abstract ResourceType ResourceType { get; }

    [NotNull]
    protected T1 AssertResource<T1>(T1 resource) where T1 : class
    {
      if (resource == null)
      {
        switch (ResourceType)
        {
          case ResourceType.Database:
            throw new DatabaseResourceNotAvailableException();

          case ResourceType.LogFiles:
            throw new LogFilesResourceNotAvailableException();

          case ResourceType.WebServer:
            throw new WebServerResourceNotAvailableException();

          default:
            throw new ResourceNotAvailableException(ResourceType);
        }
      }

      return resource;
    }

    [NotNull]
    protected T1 AssertResource<T1>(T1 resource, string details) where T1 : class
    {
      if (resource == null)
      {
        switch (ResourceType)
        {
          case ResourceType.Database:
            throw new DatabaseResourceNotAvailableException(details);

          case ResourceType.LogFiles:
            throw new LogFilesResourceNotAvailableException(details);

          case ResourceType.WebServer:
            throw new WebServerResourceNotAvailableException(details);

          default:
            throw new ResourceNotAvailableException(ResourceType, details);
        }
      }

      return resource;
    }
  }
}