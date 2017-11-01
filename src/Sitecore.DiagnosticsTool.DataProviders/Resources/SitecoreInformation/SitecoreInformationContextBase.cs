namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.Collections.Generic;
  using System.Xml;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public abstract class SitecoreInformationContextBase : ResourceBase, ISitecoreInformationContext
  {
    protected readonly IServiceClient _ServiceClient;

    private IReadOnlyDictionary<string, ConfigurationFile> _AppConfigFiles;

    private IReadOnlyDictionary<string, AssemblyFile> _Assemblies;

    private XmlDocument _Configuration;

    private IReadOnlyDictionary<string, IReleaseInfo> _CorrectlyInstalledModules;

    private string _DataFolderPath;

    private string _GlobalAsaxFile;

    private IReadOnlyDictionary<string, ConfigurationFile> _IncludeFiles;

    private IReadOnlyDictionary<string, ConfigurationFile> _PrefetchFiles;

    private IReadOnlyDictionary<string, ConfigurationFile> _SecurityFiles;

    private ISitecoreDefaultsContext _SitecoreDefaults;

    private XmlDocument _SitecoreVersionXmlFile;

    private XmlDocument _WebConfigFile;

    private IModulesContext _ModulesInformation;

    private string _InstanceName;

    private ContentSearchIndexes _ContentSearchIndexes;

    protected SitecoreInformationContextBase(IServiceClient client)
    {
      _ServiceClient = client;
    }

    // for unit testing
    public IReadOnlyDictionary<string, PipelineDefinition> Pipelines { get; set; }

    public virtual IReadOnlyDictionary<string, AssemblyFile> Assemblies
    {
      get
      {
        return AssertResource(_Assemblies, "Assemblies");
      }

      set
      {
        _Assemblies = value;
      }
    }

    public virtual XmlDocument Configuration
    {
      get
      {
        return AssertResource(_Configuration, "Configuration");
      }

      set
      {
        _Configuration = value;
      }
    }

    public virtual XmlDocument WebConfigFile
    {
      get
      {
        return AssertResource(_WebConfigFile, "WebConfig file");
      }

      set
      {
        _WebConfigFile = value;
      }
    }

    public virtual string GlobalAsaxFile
    {
      get
      {
        return AssertResource(_GlobalAsaxFile, "Global.Asax File");
      }

      set
      {
        _GlobalAsaxFile = value;
      }
    }

    public virtual XmlDocument SitecoreVersionXmlFile
    {
      get
      {
        return AssertResource(_SitecoreVersionXmlFile, "Sitecore.Version.xml File");
      }

      set
      {
        _SitecoreVersionXmlFile = value;
      }
    }

    /// <summary>
    ///   The version of Sitecore extracted from the sitecore\shell\sitecore.version.xml file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    public virtual ISitecoreVersion SitecoreVersion
    {
      get
      {
        var versionFromXml = VersionHelper.GetVersionFromXml(SitecoreVersionXmlFile, _ServiceClient);

        return AssertResource(versionFromXml, "Sitecore version");
      }
    }

    public virtual ISitecoreDefaultsContext SitecoreDefaults => AssertResource(_SitecoreDefaults ?? (_SitecoreDefaults = new SitecoreDefaultsContext(SitecoreVersion, _ServiceClient)));

    public virtual IReadOnlyDictionary<string, ConfigurationFile> IncludeFiles
    {
      get
      {
        return AssertResource(_IncludeFiles, "Include Files");
      }

      protected set
      {
        _IncludeFiles = value;
      }
    }

    public string DataFolderPath
    {
      get
      {
        return AssertResource(_DataFolderPath, "Data Folder Path");
      }

      protected set
      {
        _DataFolderPath = value;
      }
    }

    public string InstanceName
    {
      get
      {
        return AssertResource(_InstanceName, nameof(InstanceName));
      }

      set
      {
        _InstanceName = value;
      }
    }

    /// <inheritdoc />
    public IDictionary<string, IContentSearchIndex> ContentSearchIndexes => _ContentSearchIndexes ?? (_ContentSearchIndexes = new ContentSearchIndexes(Configuration));

    public bool IsAnalyticsEnabled
    {
      get
      {
        if (SitecoreVersion.MajorMinorInt >= 75)
        {
          if (!GetBoolSetting("Xdb.Enabled"))
          {
            return false;
          }

          var pipeline = GetPipeline("pipelines/createTracker") ?? GetPipeline("pipelines/initializeTracker");
          if (pipeline == null)
          {
            return false;
          }
        }
        else
        {
          if (!GetBoolSetting("Analytics.Enabled"))
          {
            return false;
          }

          var pipeline = GetPipeline("pipelines/startAnalytics");
          if (pipeline == null)
          {
            return false;
          }
        }

        return true;
      }
    }

    public IModulesContext ModulesInformation => _ModulesInformation ?? (_ModulesInformation = new ModulesContext(Assemblies));

    public virtual string GetSetting(string settingName)
    {
      Assert.ArgumentNotNull(settingName, nameof(settingName));

      return ConfigurationHelper.GetSetting(Configuration, settingName, _ => SitecoreDefaults.GetSetting(settingName));
    }

    public bool GetBoolSetting(string settingName)
    {
      return bool.Parse(GetSetting(settingName));
    }

    public virtual string GetConnectionString(string connectionStringName)
    {
      Assert.ArgumentNotNull(connectionStringName, nameof(connectionStringName));

      return ConfigurationHelper.GetConnectionString(Configuration, connectionStringName);
    }

    public virtual IReadOnlyDictionary<string, PipelineDefinition> GetPipelines()
    {
      return ConfigurationHelper.GetPipelines(Configuration, Pipelines);
    }

    public IReadOnlyDictionary<string, LogicalDatabaseDefinition> GetDatabases()
    {
      return ConfigurationHelper.GetDatabases(Configuration);
    }

    public virtual PipelineDefinition GetPipeline(string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      return ConfigurationHelper.GetPipeline(Configuration, name, Pipelines);
    }

    public virtual IReadOnlyDictionary<string, ManagerDefinition> GetManagers()
    {
      return ConfigurationHelper.GetManagers(Configuration);
    }

    public virtual ManagerDefinition GetManager(string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      return ConfigurationHelper.GetManager(Configuration, name);
    }

    public IReadOnlyDictionary<string, string> GetSettings()
    {
      return ConfigurationHelper.GetSettings(Configuration);
    }
  }
}