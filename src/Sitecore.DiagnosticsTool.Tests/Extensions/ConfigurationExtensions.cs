namespace Sitecore.DiagnosticsTool.Tests.Extensions
{
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public static class ConfigurationExtensions
  {
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