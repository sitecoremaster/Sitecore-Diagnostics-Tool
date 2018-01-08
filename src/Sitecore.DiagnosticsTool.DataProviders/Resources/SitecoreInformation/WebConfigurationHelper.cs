namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.ConfigBuilder;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;

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

      // workaround for asp.net hosting doesn't let config builder work (due to appdomain)
      var configuration = new XmlDocument();
      var showConfigFile = webRoot.GetChildFile("Runtime/showconfig.xml");
      if (showConfigFile.Exists)
      {
        using (var webConfigStream = webConfigFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          configuration.Load(webConfigStream);
        }

        var sitecoreNode = configuration.SelectSingleElement("/configuration/sitecore");
        var showConfigText = showConfigFile.ReadAllText();
        var showConfigXml = new XmlDocument();
        showConfigXml.LoadXml(showConfigText);

        sitecoreNode.SetAttribute("xmlns:patch", "http://www.sitecore.net/xmlconfig/");
        sitecoreNode.InnerXml = showConfigXml.SelectSingleElement("/sitecore").InnerXml;
      }
      else
      {
        configuration = ConfigBuilder.Build(webConfigFile.FullName, true, true);
      }

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