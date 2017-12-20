namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Logging;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  /// <summary>
  ///   The SSPG data provider.
  /// </summary>
  public class SupportPackageDataProvider : IDataProvider, IDisposable
  {
    private static readonly ServiceClient client = new ServiceClient();

    private string _InstanceName;

    private IResource[] _Resources;

    [CanBeNull]
    public IReadOnlyCollection<ServerRole> Roles { get; }

    [CanBeNull]
    private Action<string> Logger { get; }

    [NotNull]
    private string TempFolderPath { get; }

    public SupportPackageDataProvider([NotNull] string packageFilePath, [CanBeNull] IReadOnlyCollection<ServerRole> roles, [CanBeNull] Action<string> logger)
    {
      SourcePath = packageFilePath;
      FileName = Path.GetFileName(packageFilePath);
      Roles = roles;
      Logger = logger;
      Assert.ArgumentNotNull(packageFilePath, nameof(packageFilePath));

      if (File.Exists(packageFilePath))
      {
        TempFolderPath = Path.GetTempFileName() + "dt";
        if (!Directory.Exists(TempFolderPath))
        {
          Directory.CreateDirectory(TempFolderPath);
        }

        logger?.Invoke($"Unpacking {packageFilePath} to {TempFolderPath}");
        UnpackZip(packageFilePath, TempFolderPath);
      }
      else if (Directory.Exists(packageFilePath))
      {
        TempFolderPath = packageFilePath;
      }
      else
      {
        throw new InvalidOperationException("Neither file nor folder exists: " + packageFilePath);
      }
    }

    public string SourcePath { get; }

    [NotNull]
    public string FileName { get; }

    public IEnumerable<IResource> GetResources()
    {
      return _Resources ?? (_Resources = DoGetResources().ToArray());
    }

    public IEnumerable<IResource> DoGetResources()
    {
      var rootPath = GetRootPath();

      var instanceName = FileName;
      
      Logger?.Invoke("Parsing log files...");
      var logFolder = Path.GetDirectoryName(Directory.GetFiles(TempFolderPath, "log*.txt", SearchOption.AllDirectories).OrderBy(x => x.Length).FirstOrDefault());

      var logging = LoggingContext.ParseFolder(logFolder);
      if (logging != null)
      {
        yield return logging;

        if (instanceName == FileName)
        {
          var files = Directory.GetFiles(logFolder, "log.*.txt", SearchOption.AllDirectories).OrderByDescending(x => x); // start with last files first
          foreach (var file in files)
          {
            // in first 100 lines Sitecore must write "Sitecore started", otherwise we can skip this file
            var lines = File.ReadLines(file).Take(100);
            if (!lines.Any(x => x.Contains("Sitecore started")))
            {
              continue;
            }

            lines = File.ReadLines(file).Take(2000);

            // in first 2K rows there must be:
            // - assemblies e.g. C:\path\to\file.dll (AssemblyProduct, AssemblyDescription, AssemblyInformationalVersion)
            // - Operating system Microsoft Windows NT 6.3.9600.0
            // - UTC offset: -04:00:00
            // - Machine name: WS-ANP
            // - App pool ID: sc82pool
            // - Process ID: 9844
            // - Windows identity used by the process: DK\ANP. Impersonation: False
            // - Managed pipeline mode: Integrated
            // - EventQueues enabled: True
            // - Instance Name:WS-ANP-sc82
            // - Databases
            // -   core
            // -   master
            // -   web
            // -   filesystem
            // - Heartbeat - Initializing
            // - Heartbeat - Interval set to: 00:00:02
            // - Heartbeat - Worker thread started
            // - Trying to load XML configuration /App_Config/Security/Domains.config
            // -   sitecore
            // -   extranet
            // -   ds
            // -   default
            // - xDB is disabled.
            // - Tracking is disabled.
            // - Missing valid xDB license.

            var token = "Instance Name:";
            instanceName = lines.FirstOrDefault(line => line.Contains(token));
            instanceName = instanceName.Substring(instanceName.IndexOf(token) + token.Length).Trim();

            // stop parsing files
            break;
          }
        }
      }

      if (instanceName == FileName)
      {
        // if log files does not contain instance name - use package metadata
        var instanceInfoPath = Directory.GetFiles(TempFolderPath, "InstanceInfo.xml", SearchOption.AllDirectories).FirstOrDefault();
        if (instanceInfoPath == null || !File.Exists(instanceInfoPath))
        {
          instanceInfoPath = Directory.GetFiles(TempFolderPath, "PackageInfo.xml", SearchOption.AllDirectories).FirstOrDefault();
        }

        if (instanceInfoPath != null && File.Exists(instanceInfoPath))
        {
          var xml = new XmlDocument();
          xml.Load(instanceInfoPath);

          var name = xml.SelectSingleElement("package/selectedInstance")?.InnerText;
          if (!string.IsNullOrEmpty(name))
          {
            instanceName = name;

            var machineName = xml.SelectSingleElement("package/machineName")?.InnerText;
            if (!string.IsNullOrWhiteSpace(machineName))
            {
              instanceName = machineName + "-" + instanceName;
            }
          }
        }
      }

      Logger?.Invoke("Parsing Sitecore information...");

      var info = SitecoreInformationContext.TryParse(TempFolderPath, instanceName, client);
      Assert.IsNotNull(info, nameof(info));

      yield return info;

      yield return new ServerRolesContext(Roles ?? ParseRoles(info));

      Logger?.Invoke("Parsing database data...");
      var databases = SitecorePackageDatabaseContext.TryParse(rootPath, info);
      if (databases != null)
      {
        yield return databases;
      }

      var webServer = SupportPackageWebServerContext.Parse(rootPath);
      if (webServer != null)
      {
        yield return webServer;
      }
    }

    private ServerRole[] ParseRoles(SitecoreInformationContext info)
    {
      var value = info.Configuration
        .SelectSingleElement("/configuration/appSettings/add[@key='role:define']")?.GetAttribute("value");

      var roles = new List<ServerRole>();
      if (!string.IsNullOrEmpty(value))
      {
        foreach (var text in value.Split(",;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
          var name = text.Trim();

          ServerRole role;
          if (Enum.TryParse(name, out role))
          {
            roles.Add(role);
          }
          else if (name.Equals("Standalone", StringComparison.OrdinalIgnoreCase))
          {
            return Enum.GetValues(typeof(ServerRole)).Cast<ServerRole>().ToArray();
          }
          else
          {
            Logger?.Invoke($"Couldn't parse role: {name}");
          }
        }

        return roles.ToArray();
      }

      throw new InvalidOperationException("The roles are not specified in the role:define attribute in the web.config file");
    }

    public void UnpackZip([NotNull] string packagePath, [NotNull] string folder)
    {
      Assert.ArgumentNotNull(packagePath, nameof(packagePath));
      Assert.ArgumentNotNull(folder, nameof(folder));

      try
      {
        ZipFile.ExtractToDirectory(packagePath, folder);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"The \"{packagePath}\" package seems to be corrupted.", ex);
      }
    }

    private string GetRootPath()
    {
      var rootPath = TempFolderPath;

      if (Directory.GetFiles(rootPath).Length > 0)
      {
        return rootPath;
      }

      var subDirs = Directory.GetDirectories(rootPath);
      if (subDirs.Length == 1)
      {
        return subDirs.Single();
      }

      return rootPath;
    }

    public void Dispose()
    {
      if (SourcePath != TempFolderPath)
      {
        Directory.Delete(TempFolderPath, true);
      }
    }

    public string InstanceName
    {
      get
      {
        return _InstanceName ?? (_InstanceName = GetResources().OfType<ISitecoreInformationContext>().FirstOrDefault()?.InstanceName);
      }

      set
      {
        throw new NotImplementedException();
      }
    }
  }
}