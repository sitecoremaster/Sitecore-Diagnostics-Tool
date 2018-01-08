namespace Sitecore.DiagnosticsTool.Tests.ECM.Helpers
{
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;

  internal static class EcmHelper
  {
    [NotNull]
    public static string GetAttributeValue([NotNull] XmlDocument configuration, [NotNull] string nodeXPath, [NotNull] string attributeName)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(nodeXPath, nameof(nodeXPath));
      Assert.ArgumentNotNull(attributeName, nameof(attributeName));

      return configuration.SelectElements(nodeXPath).LastOrDefault().With(x => x.Attributes[attributeName].With(a => a.Value) ?? x.InnerText);
    }

    public static ISitecoreVersion GetEcmVersion([NotNull] IInstanceResourceContext data)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      return data.SitecoreInfo.ModulesInformation.InstalledModules.TryGetValue("Email Experience Manager")?.Release.Version;
    }
  }
}