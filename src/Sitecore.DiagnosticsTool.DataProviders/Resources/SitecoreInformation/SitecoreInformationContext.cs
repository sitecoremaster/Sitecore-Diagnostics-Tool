namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  public class SitecoreInformationContext : SitecoreInformationContextBase
  {
    private static readonly IServiceClient _DefaultServiceClient = new ServiceClient();

    public SitecoreInformationContext()
      : base(_DefaultServiceClient)
    {
    }

    public SitecoreInformationContext(IServiceClient client)
      : base(client)
    {
    }

    protected override ResourceType ResourceType => ResourceType.SitecoreInformation;

    [CanBeNull]
    public static SitecoreInformationContext TryParse([NotNull] string rootPath, string instanceName, IServiceClient client)
    {
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      Assert.ArgumentCondition(Directory.Exists(rootPath), nameof(rootPath), $"The {nameof(rootPath)} directory does not exist.");

      var sitecoreVersionXmlPath = FindFile(rootPath, "sitecore.version.xml");
      if (sitecoreVersionXmlPath == null || !File.Exists(sitecoreVersionXmlPath))
      {
        return null;
      }

      // version
      XmlDocument sitecoreVersionXml = null;
      Log.Info("Sitecore.version.xml = " + sitecoreVersionXmlPath);
      Log.Info("Parsing XML from sitecore.version.xml file");
      sitecoreVersionXml = new XmlDocument().TryLoadFile(sitecoreVersionXmlPath);

      try
      {
        Log.Info("Sitecore version: " + VersionHelper.GetVersionFromXml(sitecoreVersionXml, client));
      }
      catch (Exception)
      {
        Log.Error("Sitecore version cannot be retrieved from: " + sitecoreVersionXml.OuterXml);
      }

      // configs
      XmlDocument webConfigFile = null;
      XmlDocument resultConfiguration = null;
      Dictionary<string, ConfigurationFile> includeFiles = null;

      string dataFolderPath = null;
      var appConfig = Find(rootPath, "App_Config");
      if (appConfig != null)
      {
        var webRootPath = Path.GetDirectoryName(appConfig);
        Log.Info("WebRootPath = " + webRootPath);

        foreach (var filePath in Directory.GetFiles(appConfig, "*", SearchOption.AllDirectories))
        {
          Log.Info("Config File = " + filePath);
        }

        var webConfigPath = Path.Combine(webRootPath, "web.config");
        Log.Info("WebConfig = " + webConfigPath);

        resultConfiguration = WebConfigurationHelper.GetConfiguration(webRootPath);
        Assert.IsNotNull(resultConfiguration, "resultConfiguration");

        dataFolderPath = GetDataFolderPath(webRootPath, resultConfiguration);
        if (dataFolderPath == null || !Directory.Exists(dataFolderPath))
        {
          dataFolderPath = null;
        }

        Log.Info("Parsing XML from web.config file");
        webConfigFile = new XmlDocument().TryLoadFile(webConfigPath);

        Log.Info("Parsing App_Config files");
        var appConfigFiles = Directory.GetFiles(Path.Combine(webRootPath, "App_Config"), "*.config", SearchOption.AllDirectories)
          .ToDictionary(file => file.Substring(webRootPath.Length, file.Length - webRootPath.Length), file => new ConfigurationFile(file, File.ReadAllText(file)));

        Log.Info("Searching for \\App_Config\\Sitecore, Modules, Include, Environment *.config files");
        includeFiles = FilterFiles(appConfigFiles, "\\App_Config\\Sitecore\\", "\\App_Config\\Modules\\", "\\App_Config\\Include\\", "\\App_Config\\Environment\\");
      }

      var globalAsaxPath = FindFile(rootPath, "global.asax");
      string globalAsaxFile = null;
      if (globalAsaxPath != null && File.Exists(globalAsaxPath))
      {
        Log.Info("Global.asax = " + globalAsaxPath);
        Log.Info("Reading Global.asax");
        using (var sr = File.OpenText(globalAsaxPath))
        {
          globalAsaxFile = sr.ReadToEnd();
          Log.Info("Global.asax: \r\n" + globalAsaxFile);
        }
      }

      var informationContext = new SitecoreInformationContext(client)
      {
        InstanceName = instanceName,
        WebConfigFile = webConfigFile,
        DataFolderPath = dataFolderPath,
        GlobalAsaxFile = globalAsaxFile,
        Configuration = resultConfiguration,
        IncludeFiles = includeFiles,
        SitecoreVersionXmlFile = sitecoreVersionXml,
        Assemblies = new AssemblyFileCollection(GetBinaries(rootPath) ?? new AssemblyFile[0])
      };

      return informationContext;
    }

    private static string Find(string rootPath, string name)
    {
      var dir = Directory.GetDirectories(rootPath, name, SearchOption.AllDirectories).OrderBy(x => x.Length).FirstOrDefault();
      if (dir == null)
      {
        Log.Warn($"The {name} is not found anywhere in {rootPath}");
      }

      return dir;
    }

    private static string FindFile(string rootPath, string name)
    {
      var dir = Directory.GetFiles(rootPath, name, SearchOption.AllDirectories).OrderBy(x => x.Length).FirstOrDefault();
      if (dir == null)
      {
        Log.Warn($"The {name} is not found anywhere in {rootPath}");
      }

      return dir;
    }

    private static string GetDataFolderPath([NotNull] string webRootPath, [NotNull] XmlDocument configuration)
    {
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      var dataFolderPath = configuration
        .SelectNodes("/configuration/sitecore/sc.variable[@name='dataFolder']")
        .OfType<XmlElement>()
        .LastOrDefault()
        .With(x => x.GetAttribute("value"));

      if (!string.IsNullOrEmpty(dataFolderPath))
      {
        return dataFolderPath;
      }

      return Path.Combine(webRootPath, "data");
    }

    private static IEnumerable<AssemblyFile> GetBinaries([NotNull] string rootPath)
    {
      Assert.ArgumentNotNull(rootPath, nameof(rootPath));
      try
      {
        // in 9.0 sitecore.kernel.dll is always present in support package, so check Client.dll
        var kernel = FindFile(rootPath, "Sitecore.Client.dll");
        if (kernel == null)
        {
          var assemblyInfoPath = FindFile(rootPath, "AssemblyInfo.xml");
          if (assemblyInfoPath != null && File.Exists(assemblyInfoPath))
          {
            var xml = new XmlDocument().TryLoadFile(assemblyInfoPath);
            return xml.SelectElements("/assemblies/assembly").Select(ParseAssembly).GroupBy(x => x.FileName).ToDictionary(x => x.Key, x => x.First()).Values; // GroupBy to bypass duplication issues
          }

          return null;
        }

        var binPath = Path.GetDirectoryName(kernel);
        var files = Directory.GetFiles(binPath, "*.dll", SearchOption.AllDirectories);

        var binaries = new Dictionary<string, AssemblyFile>();
        foreach (var file in files)
        {
          try
          {
            var name = file.Substring(binPath.Length).TrimStart("\\/".ToCharArray());
            var assembly = new AssemblyFile(new FileInfo(file));
            if (name != null)
            {
              Log.Info("Assembly detected: " + file);
              binaries.Add(name, assembly);
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception, $"Error happened during parsing assembly. Skipping: {file}");
          }
        }

        return binaries.Values;
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"Unhandled error happened during parsing binaries: {rootPath}");

        return null;
      }
    }

    private static AssemblyFile ParseAssembly(XmlElement assembly)
    {
      try
      {
        var name = assembly.GetAttribute("name");
        var fileVersion = assembly["fileVersion"].InnerText.Replace(", ", ".");
        var productVersion = assembly["productVersion"].InnerText.Replace(", ", ".");
        var md5 = assembly["md5"].InnerText;
        var fileSize = assembly["fileSize"]?.InnerText;
        var fileSizeLong = fileSize == null ? null : (long?)long.Parse(fileSize);
        var lastModified = assembly["lastWriteTime"]?.InnerText;

        //TODO: enable that when SSPG saves date in ISO format var lastUpdated = DateTime.Parse(assembly["lastWriteTime"].InnerText);
        //TODO: implement md5

        return new AssemblyFile(name, fileVersion, productVersion, fileSizeLong, null, md5);
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"Failed to parse assembly from xml: {assembly.OuterXml}");

        return null;
      }
    }

    [NotNull]
    private static Dictionary<string, ConfigurationFile> FilterFiles([NotNull] Dictionary<string, ConfigurationFile> appConfigFiles, [NotNull] params string[] filter)
    {
      Assert.ArgumentNotNull(appConfigFiles, nameof(appConfigFiles));
      Assert.ArgumentNotNull(filter, nameof(filter));

      return appConfigFiles.Where(item => filter.Any(f => item.Key.StartsWith(f))).ToDictionary(item => item.Key, item => item.Value);
    }
  }
}