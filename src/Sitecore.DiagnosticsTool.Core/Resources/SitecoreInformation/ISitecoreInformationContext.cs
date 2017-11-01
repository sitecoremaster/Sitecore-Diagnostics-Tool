namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.Core.Resources.Modules;

  /// <summary>
  ///   The read-only interface for accessing Sitecore assemblies and configuration files
  /// </summary>
  public interface ISitecoreInformationContext : ISitecoreConfigurationContext, ISitecoreAssembliesContext, IResource, IInstanceName
  {
    /// <summary>
    ///   The original web.config file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    XmlDocument WebConfigFile { get; }

    /// <summary>
    ///   The data folder path.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    string DataFolderPath { get; }

    /// <summary>
    ///   The original Global.asax file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    string GlobalAsaxFile { get; }

    /// <summary>
    ///   The sitecore\shell\sitecore.version.xml file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    XmlDocument SitecoreVersionXmlFile { get; }

    /// <summary>
    ///   The version of Sitecore extracted from the sitecore\shell\sitecore.version.xml file
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ISitecoreVersion SitecoreVersion { get; }

    /// <summary>
    ///   The defaults of the Sitecore version extracted from the sitecore\shell\sitecore.version.xml file.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    ISitecoreDefaultsContext SitecoreDefaults { get; }

    /// <summary>
    ///   All the App_Config\Include\*.config files
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IReadOnlyDictionary<string, ConfigurationFile> IncludeFiles { get; }

    ///// <summary>
    ///// All configuration files that were merged into web.config file while initialization. 
    ///// For example: 
    ///// - App_Config\ConnectionStrings.config
    ///// - App_Config\Include\Sitecore.Analytics.config
    ///// 
    ///// Note that some files that you might expect to see here are not included into this collection because they are processed at runtime:
    ///// - App_Config\Security\Domain.config
    ///// - App_Config\Security\GlobalRoles.config
    ///// </summary>
    ///// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    //[NotNull]
    //[PublicAPI]
    //IDictionary<string, XmlDocument> ConfigurationFiles { get; }

    /// <summary>
    ///   Information about Sitecore modules.
    /// </summary>
    [NotNull]
    IModulesContext ModulesInformation { get; }

    /// <summary>
    ///   Information about ContentSearch indexes.
    /// </summary>
    IDictionary<string, IContentSearchIndex> ContentSearchIndexes { get; }

    /// <summary>
    ///   Checks if necessary pipelines are presented and appropriate setting is turned on.
    /// </summary>
    bool IsAnalyticsEnabled { get; }
  }
}