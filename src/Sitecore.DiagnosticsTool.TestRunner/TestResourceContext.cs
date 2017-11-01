namespace Sitecore.DiagnosticsTool.TestRunner
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Resources.FileSystem;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public sealed class TestResourceContext : ITestResourceContext
  {
    private ISystemContext _System;

    private IServerRolesContext _ServerRoleses;

    private IDatabaseContext _Databases;

    private IFileSystemContext _FileSystem;

    private ILoggingContext _Logs;

    private ISitecoreInformationContext _SitecoreInfo;

    private IWebServerContext _WebServer;

    public TestResourceContext(string instanceName)
    {
      InstanceName = instanceName;
    }

    public string InstanceName { get; set; }

    public IServerRolesContext ServerRoles
    {
      get
      {
        return AssertResource(_ServerRoleses, ResourceType.SitecoreInformation, "Categories");
      }

      [UsedImplicitly]
      set
      {
        _ServerRoleses = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing File system
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public IFileSystemContext FileSystem
    {
      get
      {
        return AssertResource(_FileSystem, ResourceType.FileSystem);
      }

      [UsedImplicitly]
      set
      {
        _FileSystem = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing Web Server (IIS)
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public IWebServerContext WebServer
    {
      get
      {
        return AssertResource(_WebServer, ResourceType.WebServer);
      }

      [UsedImplicitly]
      set
      {
        _WebServer = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing Sitecore assemblies and configuration files
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public ISitecoreInformationContext SitecoreInfo
    {
      get
      {
        return AssertResource(_SitecoreInfo, ResourceType.SitecoreInformation);
      }

      [UsedImplicitly]
      set
      {
        _SitecoreInfo = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing Sitecore and IIS Log files
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public ILoggingContext Logs
    {
      get
      {
        return AssertResource(_Logs, ResourceType.LogFiles);
      }

      [UsedImplicitly]
      set
      {
        _Logs = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing Sitecore databases
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public IDatabaseContext Databases
    {
      get
      {
        return AssertResource(_Databases, ResourceType.Database);
      }

      [UsedImplicitly]
      set
      {
        _Databases = value;
      }
    }

    /// <summary>
    ///   The read-only interface for accessing Sitecore databases
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public ISystemContext System
    {
      get
      {
        return AssertResource(_System, ResourceType.System);
      }

      [UsedImplicitly]
      set
      {
        _System = value;
      }
    }

    [NotNull]
    protected T1 AssertResource<T1>(T1 resource, ResourceType resourceType, string details = null)
      where T1 : class
    {
      if (resource == null)
      {
        if (string.IsNullOrEmpty(details))
        {
          throw new ResourceNotAvailableException(resourceType);
        }

        throw new ResourceNotAvailableException(resourceType, details);
      }

      return resource;
    }
  }
}