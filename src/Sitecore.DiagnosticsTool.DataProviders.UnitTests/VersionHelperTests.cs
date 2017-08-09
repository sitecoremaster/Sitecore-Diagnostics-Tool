namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using FluentAssertions;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;
  using Xunit;

  public class VersionHelperTests
  {
    [Theory]
    public void InputDataIsReturned()
    {
      var random = new Random();
      var ver = new Version(random.Next(), random.Next());
      int hotfix = random.Next();
      var productName = "Sitecore CMS";
      var hotfixText = $"Hotfix {hotfix}-2";
      var version = new SitecoreVersion(ver, hotfixText);
      var releases = new Dictionary<string, IRelease>
      {
        { ver.Revision.ToString(), new Release(productName, version, "whatever", DateTime.MinValue, new Dictionary<string, IDistribution>(), new ReleaseCompatibility()) }
      };
      var productVersions = new Dictionary<string, IProductVersion>
      {
        { version.MajorMinor, new ProductVersion(version.MajorMinor, releases) }
      };

      var client = new MockServiceClient
      {
        new Product(productName, productVersions, new ProductMetadata())
      };

      var doc = new XmlDocument();
      doc.LoadXml($"<information><version><major>{ver.Major}</major><minor>{ver.Minor}</minor><build>0</build><revision>{ver.Revision} {hotfixText}</revision></version></information>");
      var sut = VersionHelper.GetVersionFromXml(doc, client);

      sut.MajorMinorUpdate
        .Should().Be($"{ver.Major}.{ver.Minor}.{ver.Build}");

      sut.Revision
        .Should().Be(ver.Revision);

      sut.Hotfix
        .Should().Be(hotfixText);
    }
  }
}