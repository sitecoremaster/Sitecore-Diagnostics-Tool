namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.IO;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.ConfigBuilder;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.Diagnostics.Logging;

  public static class WebConfigurationHelper
  {
    public static XmlDocument GetConfiguration([NotNull] IDirectory webRoot)
    {
      Assert.ArgumentNotNull(webRoot, nameof(webRoot));
      var webConfigFile = webRoot.GetChildFile("web.config");

      if (!webConfigFile.Exists)
      {
        return null;
      }

      var configuration = ConfigBuilder.Build(webConfigFile.FullName, true, true);
      var configurationElement = (XmlElement)configuration.SelectSingleNode("/configuration");
      Assert.IsNotNull(configurationElement, "configurationElement");

      var locations = configuration.SelectNodes("/configuration/location").OfType<XmlElement>();
      foreach (var location in locations)
      {
        var path = location.GetAttribute("path");
        if (!string.IsNullOrEmpty(path) && !path.Equals("/"))
        {
          // we can ignore non-global locations
          Log.Warn($"The configuration contains <location path=\"{path}\">");
          continue;
        }

        Log.Warn("The configuration contains <location>, which is being moved to <configuration>");
        var children = location.ChildNodes.OfType<XmlElement>().ToArray();
        foreach (var child in children)
        {
          location.RemoveChild(child);
          var localname = child.Name;
          if (configurationElement[localname] == null)
          {
            configurationElement.AppendChild(child);
          }
          else
          {
            Log.Error(string.Format("There is a conflict between /configuration/location/{0} and /configuration/{0}. The element is ignored.", localname));
          }
        }
      }

      return configuration;
    }
  }
}