namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.IO;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.ConfigBuilder;
  using Sitecore.Diagnostics.Logging;

  public static class WebConfigurationHelper
  {
    public static XmlDocument GetConfiguration([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));
      var webConfigPath = Path.Combine(webRootPath, "web.config");

      if (!File.Exists(webConfigPath))
      {
        return null;
      }

      var configuration = ConfigBuilder.Build(webConfigPath, true, true);
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