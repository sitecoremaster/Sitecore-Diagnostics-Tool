namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;

  public static class VersionHelper
  {
    [NotNull]
    public static ISitecoreVersion GetVersionFromXml([NotNull] XmlDocument sitecoreVersionXml, IServiceClient client)
    {
      Assert.ArgumentNotNull(sitecoreVersionXml, nameof(sitecoreVersionXml));

      var majorNode = sitecoreVersionXml.SelectSingleNode(@"/information/version/major");
      Assert.IsNotNull(majorNode, nameof(majorNode));

      var minorNode = sitecoreVersionXml.SelectSingleNode(@"/information/version/minor");
      Assert.IsNotNull(minorNode, nameof(minorNode));

      var revNode = sitecoreVersionXml.SelectSingleNode(@"/information/version/revision");
      Assert.IsNotNull(revNode, nameof(revNode));

      var major = int.Parse(majorNode.InnerText);
      var minor = int.Parse(minorNode.InnerText);
      var revisionText = revNode.InnerText;
      var pos = revisionText.IndexOf(' ');
      var hotfixText = pos >= 0 ? revisionText.Substring(Math.Min(pos + 1, revisionText.Length - 1)) : null;
      revisionText = pos >= 0 ? revisionText.Substring(0, pos) : revisionText;

      var productName = "Sitecore CMS";
      var versionText = $"{major}.{minor}";
      var release = client.GetRelease(productName, versionText, revisionText);
      var update = release.Version.Update;
      var revision = int.Parse(revisionText);

      if (string.IsNullOrEmpty(hotfixText))
      {
        return new SitecoreVersion(major, minor, update, revision);
      }

      return new SitecoreVersion(major, minor, update, revision, hotfixText);
    }
  }
}