namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  public static class ConfigurationHelper
  {
    [NotNull]
    public static IReadOnlyDictionary<string, string> GetSettings([NotNull] XmlDocument configuration)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      return configuration
        .SelectElements("/configuration/sitecore/settings/setting")
        .ToDictionary(x => x.GetAttribute("name"), x => x.Attributes["value"].With(a => a.Value) ?? x.InnerText);
    }

    [NotNull]
    public static string GetSetting([NotNull] XmlDocument configuration, [NotNull] string settingName, [CanBeNull] Func<string, string> defaultValue = null)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(settingName, nameof(settingName));

      return configuration
          .SelectElements($"/configuration/sitecore/settings/setting[@name='{settingName}']")
          .LastOrDefault() // take last setting as it is effective
          .With(x => ParseSettingValue(x)) // if not null then get "value" attribute
          ?? defaultValue?.Invoke(settingName)
          ?? string.Empty;
    }

    [CanBeNull]
    public static string GetConnectionString([NotNull] XmlDocument configuration, [NotNull] string connectionStringName)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(connectionStringName, nameof(connectionStringName));

      return configuration
        .SelectElements($"/configuration/connectionStrings/add[@name='{connectionStringName}']")
        .LastOrDefault() // take last setting as it is effective
        .With(x => x.GetAttribute("connectionString")); // if not null then get "connectionString" attribute
    }

    private static string ParseSettingValue([NotNull] XmlElement setting)
    {
      Assert.ArgumentNotNull(setting, nameof(setting));

      var attr = setting.Attributes["value"];
      if (attr != null)
      {
        return attr.Value;
      }

      return setting.InnerText;
    }

    [NotNull]
    public static IReadOnlyDictionary<string, PipelineDefinition> GetPipelines([NotNull] XmlDocument configuration, [CanBeNull] IReadOnlyDictionary<string, PipelineDefinition> pipelinesOverride = null)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      return pipelinesOverride ?? GetPipelinesInner(configuration);
    }

    /// <param name="configuration"></param>
    /// <param name="name">The name of pipeline with group prefix (for example, pipelines/itemProvider/addFromTemplate)</param>
    /// <param name="pipelinesOverride">The pipelines set to override.</param>
    public static PipelineDefinition GetPipeline([NotNull] XmlDocument configuration, [NotNull] string name, [CanBeNull] IReadOnlyDictionary<string, PipelineDefinition> pipelinesOverride = null)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentCondition(name.Contains('/'), nameof(name), $"The name must contain slash to identify the group of pipelines (actual: {name})");

      return GetPipelines(configuration, pipelinesOverride).TryGetValue(name);
    }

    public static ManagerDefinition GetManager([NotNull] XmlDocument configuration, [NotNull] string name)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(name, nameof(name));

      var manager = configuration.SelectSingleNode("/configuration/sitecore/" + name) as XmlElement;
      if (manager != null)
      {
        return ManagerDefinition.Parse(manager);
      }

      return null;
    }

    [NotNull]
    public static IReadOnlyDictionary<string, ManagerDefinition> GetManagers([NotNull] XmlDocument configuration)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      return configuration
        .SelectElements("/configuration/sitecore/*/providers")
        .Select(p => (XmlElement)p.ParentNode)
        .Select(m => ManagerDefinition.Parse(m))
        .ToDictionary(m => m.Name, x => x);
    }

    [NotNull]
    private static IReadOnlyDictionary<string, PipelineDefinition> GetPipelinesInner([NotNull] XmlDocument configuration)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      var paths = new[]
      {
        // CorePipeline.Run(...) represents /configuration/sitecore/processors/<pipeline>/processor[@type='...']
        "/configuration/sitecore/pipelines/*",
        "/configuration/sitecore/pipelines/group/pipelines/*",

        // Pipeline.Run(...) represents /configuration/sitecore/processors/<pipeline>/processor[@type='...']
        "/configuration/sitecore/processors/*",
      };

      var result = new Dictionary<string, PipelineDefinition>();
      foreach (var path in paths)
      {
        var prefix = path.Substring("/configuration/sitecore/".Length)
          .Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
          .First();

        var corePipelines = configuration.SelectElements(path);
        foreach (var pipeline in corePipelines)
        {
          if (pipeline.Name == "group")
          {
            continue;
          }

          var definition = PipelineDefinition.Parse(pipeline, prefix);
          if (result.ContainsKey(definition.Name))
          {
            Log.Error($"Pipeline with same name already exists: {pipeline.Name}, Xml: {pipeline.OuterXml}");

            continue;
          }

          result.Add(definition.Name, definition);
        }
      }

      return result;
    }
  }
}