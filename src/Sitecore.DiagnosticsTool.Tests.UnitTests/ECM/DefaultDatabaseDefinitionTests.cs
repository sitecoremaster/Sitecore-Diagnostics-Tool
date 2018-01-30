namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.ECM;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  using Xunit;
  using Diagnostics.InfoService.Client.Builder;

  public class DefaultDefinitionDatabaseTests : DefaultDefinitionDatabase
  {
    [Fact]
    public void TestPassed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        InstanceName="cd1",
        ServerRoles = new[] { ServerRole.ContentDelivery },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings/setting[@name='Analytics.DefaultDefinitionDatabase' and @value='web']"),
        Version = new SitecoreVersion(8, 2, 3),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
        { new ReleaseInfo(ServiceClientBuilder.Create().Build().Products["Email Experience Manager"].Versions["3.4.1"]) }
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
        InstanceName = "cd1",
        ServerRoles = new[] { ServerRole.ContentDelivery },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings"), //no setting means "master" database is used.
        Version = new SitecoreVersion(8, 2, 2),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
          { new ReleaseInfo(ServiceClientBuilder.Create().Build().Products["Email Experience Manager"].Versions["3.4.1"]) }
        },
      };

      new SolutionUnitTestContext()

        .AddInstance(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ErrorMessage))
        .Done();
    }
  }
}