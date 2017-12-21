namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Xml;

  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.ECM;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class RendererUrlSettingTests : RendererUrlSetting
  {
    [Fact]
    public void TestPassed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        ServerRoles = new[] { ServerRole.ContentManagement },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings/setting[@name='ECM.RendererUrl' and @value='http://sitecore.net']"),
        Version = new SitecoreVersion(7, 2, 2),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
          { new ReleaseInfo(new ServiceClient().Products["Email Experience Manager"].Versions["2.2.0"]) }
        },
      };

      new SolutionUnitTestContext()
        
        .AddInstance(sitecoreConfiguration)
        .Process(this)
        .Done();
    }

    [Fact]
    public void TestFailed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        ServerRoles = new[] { ServerRole.ContentManagement },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings"),
        Version = new SitecoreVersion(7, 2, 2),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
          { new ReleaseInfo(new ServiceClient().Products["Email Experience Manager"].Versions["2.2.0"]) }
        },
      };

      new SolutionUnitTestContext()
        
        .AddInstance(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormatWithMessage, KbNumber, ErrorMessage, KbName), Link))
        .Done();
    }
  }
}