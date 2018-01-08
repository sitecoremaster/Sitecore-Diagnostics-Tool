namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Remoting;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.InfoService.Client.Model.Defaults;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class SitecoreDefaultsContext : ISitecoreDefaultsContext
  {
    private static readonly IServiceClient _DefaultServiceClient = new ServiceClient();

    private const string ContactSupportMessage = "This is remote information service malfunction, please try again later. If the error is persistent, please notify Sitecore Support team.";

    [NotNull]
    private readonly IServiceClient _Client;

    [NotNull]
    private IRelease Release { get; }

    private IReadOnlyDictionary<string, AssemblyFile> _Assemblies;

    private XmlDocument _Configuration;

    private IReadOnlyDictionary<string, IReleaseDefaultSqlDatabase> _Databases;

    private IDistributionDefaults _Defaults;

    private List<ISitecoreModuleInfo> _ModulesInformation;

    public SitecoreDefaultsContext([NotNull] ISitecoreVersion sitecoreVersion, IServiceClient client = null)
    {
      Assert.ArgumentNotNull(sitecoreVersion, nameof(sitecoreVersion));

      Log.Info($"Initializing defaults context for {sitecoreVersion}");

      _Client = client ?? _DefaultServiceClient;

      try
      {
        Release = _Client.Products["Sitecore CMS"].Versions[sitecoreVersion.MajorMinorUpdate];
      }
      catch (Exception ex)
      {
        throw new ResourceNotAvailableException(ResourceType.SitecoreInformation, $"SitecoreDefaults (cannot find {sitecoreVersion} release). " + ContactSupportMessage, ex);
      }
    }

    [NotNull]
    private IDistributionDefaults Defaults => _Defaults ?? (_Defaults = GetDefaults());

    public virtual XmlDocument Configuration => _Configuration ?? (_Configuration = GetConfiguration());

    public IReadOnlyDictionary<string, AssemblyFile> Assemblies => _Assemblies ?? (_Assemblies = GetAssemblies());

    public virtual IReadOnlyDictionary<string, IReleaseDefaultSqlDatabase> SqlDatabases => _Databases ?? (_Databases = GetSqlDatabases());

    // for unit testing
    public IReadOnlyDictionary<string, PipelineDefinition> Pipelines { get; set; }

    public IReadOnlyDictionary<string, string> GetSettings()
    {
      return ConfigurationHelper.GetSettings(Configuration);
    }

    public virtual string GetSetting(string settingName, string defaultValue = null)
    {
      Assert.ArgumentNotNull(settingName, nameof(settingName));

      return ConfigurationHelper.GetSetting(Configuration, settingName, _ => defaultValue);
    }

    public bool GetBoolSetting(string settingName, bool? defaultValue = null)
    {
      return bool.Parse(GetSetting(settingName, defaultValue?.ToString()));
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

    private XmlDocument GetConfiguration()
    {
      var release = Release;

      Log.Info($"Initializing defatult configuration for {release}");
      var configuration = Defaults.Configs.Configuration;
      Assert.IsNotNull(configuration, $"Configuration is not available for {release.ProductName} {release.Version}. " + ContactSupportMessage);

      Log.Info("Configuration: \r\n" + configuration.OuterXml);

      return configuration;
    }

    private IReadOnlyDictionary<string, AssemblyFile> GetAssemblies()
    {
      var release = Release;
      Log.Info($"Initializing defatult assemblies for {release}");

      var assemblies = Defaults.Assemblies;
      Assert.IsTrue(assemblies.Count > 0, $"Assemblies are not available for {release}. " + ContactSupportMessage);

      return assemblies;
    }

    private IReadOnlyDictionary<string, IReleaseDefaultSqlDatabase> GetSqlDatabases()
    {
      var release = Release;

      Log.Info($"Initializing defatult databases for {release}");
      var databases = Defaults.Databases;
      Assert.IsTrue(databases.Count > 0, $" SqlDatabases are not available for {release}. " + ContactSupportMessage);

      return databases.ToDictionary(x => x.Key, x => new ReleaseDefaultSqlDatabase(x.Key, x.Value) as IReleaseDefaultSqlDatabase);
    }

    private IDistributionDefaults GetDefaults()
    {
      var release = Release;
      try
      {
        Log.Info($"Initializing defaults for {release}");
        return release.DefaultDistribution.Defaults;
      }
      catch (Exception ex)
      {
        throw new RemotingException($"The this.Defaults are not available for {release}. " + ContactSupportMessage, ex);
      }
    }
  }
}