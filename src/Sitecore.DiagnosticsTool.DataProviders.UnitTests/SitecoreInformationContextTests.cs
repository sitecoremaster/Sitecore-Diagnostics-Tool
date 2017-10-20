namespace Sitecore.DiagnosticsTool.Resources.SitecoreInformation.UnitTests
{
  using System.Xml;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  [TestClass]
  public class SitecoreInformationContextTests
  {
    [TestMethod]
    public void GetSettingTest()
    {
      var context = new SitecoreInformationContext
      {
        Configuration = new XmlDocument().TryParse(@"
<configuration>
  <sitecore>
    <settings>
      <setting name=""abc"" value=""old1"" />
      <setting name=""abc"">new1</setting>
    </settings>
  </sitecore>
</configuration>")
      };

      context.GetSetting("abc")
        .Should()
        .Be("new1");
    }
  }
}