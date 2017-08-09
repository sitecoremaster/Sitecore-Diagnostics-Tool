namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using System.Collections.Generic;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  public interface ISitecoreConfigurationContext
  {
    /// <summary>
    ///   The default Sitecore Configuration of the given Sitecore version.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    XmlDocument Configuration { get; }

    /// <summary>
    ///   Retrieves Sitecore settings map from actual showconfig.
    /// </summary>
    /// <returns>Returns a dictionary with setting names and values.</returns>
    [NotNull]
    IReadOnlyDictionary<string, string> GetSettings();

    /// <summary>
    ///   Retrieves Sitecore setting value from actual showconfig.
    /// </summary>
    /// <returns>Returns a string value of the setting, or default value if the setting is not presented in the configuration.</returns>
    [NotNull]
    string GetSetting([NotNull] string settingName);

    /// <summary>
    ///   Retrieves Sitecore setting value from actual showconfig.
    /// </summary>
    /// <returns>Returns a bool value of the setting.</returns>
    bool GetBoolSetting([NotNull] string settingName);

    /// <summary>
    ///   Retrieves Sitecore connection string value from actual showconfig.
    /// </summary>
    /// <returns>Returns a string value of the connection setting, or null if it is not presented in the configuration.</returns>
    [CanBeNull]
    string GetConnectionString(string connectionStringName);

    /// <summary>
    ///   Retrieves Sitecore pipelines from actual showconfig.
    /// </summary>
    /// <returns>Returns a set of pipelines extracted from actual showconfig.</returns>
    [NotNull]
    IReadOnlyDictionary<string, PipelineDefinition> GetPipelines();

    /// <summary>
    ///   Retrieves Sitecore pipeline definition from actual showconfig if it exists.
    /// </summary>
    /// <param name="name">The name of pipeline with group prefix (for example, pipelines/itemProvider/addFromTemplate)</param>
    /// <returns>Returns pipeline definition, or null if it does not exist.</returns>
    PipelineDefinition GetPipeline([NotNull] string name);

    /// <summary>
    ///   Retrieves Sitecore Managers from actual showconfig.
    /// </summary>
    /// <returns>Returns a set of Managers extracted from actual showconfig.</returns>
    [NotNull]
    IReadOnlyDictionary<string, ManagerDefinition> GetManagers();

    /// <summary>
    ///   Retrieves Sitecore Manager definition from actual showconfig if it exists.
    /// </summary>
    /// <param name="name">The name of the Manager.</param>
    /// <returns>Returns Manager definition, or null if it does not exist.</returns>
    ManagerDefinition GetManager([NotNull] string name);
  }
}