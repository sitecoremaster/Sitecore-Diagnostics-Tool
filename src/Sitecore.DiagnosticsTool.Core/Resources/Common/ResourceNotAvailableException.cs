namespace Sitecore.DiagnosticsTool.Core.Resources.Common
{
  using System;

  using Sitecore.Diagnostics.Base;

  [Serializable]
  public class ResourceNotAvailableException : Exception
  {
    public ResourceNotAvailableException(ResourceType type)
      : base(type + " resource is not available")
    {
      Assert.ArgumentNotNull(type, nameof(type));

      ResourceType = type;
    }

    public ResourceNotAvailableException(ResourceType type, string details, Exception ex = null)
      : base($"{type} ({details}) resource is not available", ex)
    {
      Assert.ArgumentNotNull(type, nameof(type));

      ResourceType = type;
    }

    public ResourceType ResourceType { get; }
  }
}