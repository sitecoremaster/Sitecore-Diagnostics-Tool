namespace Sitecore.DiagnosticsTool.Tests.Extensions
{
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  public static class ConfigurationExtensions
  {
    [NotNull]
    [ItemNotNull]
    public static XmlElement[] GetDatabases([NotNull] this XmlDocument configuration)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));

      return configuration.SelectElements("/configuration/sitecore/databases/database");
    }

    [NotNull]
    public static string GetXPath([NotNull] this XmlNode node)
    {
      Assert.ArgumentNotNull(node, nameof(node));
      var path = string.Empty;
      while (node != null && node.GetType() != typeof(XmlDocument))
      {
        path = node.Name + "/" + path;
        node = node.ParentNode;
      }

      return path.TrimEnd('/');
    }
  }
}