namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Collections.Generic;
  using System.Xml;

  using TestRunner;
  using Core.Categories;
  using UnitTestsHelper.Context;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Tests.ECM;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Sitecore.Diagnostics.InfoService.Client.Builder;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  using Xunit;

  public class ListManagementTests : ListManagement
  {
    [Fact]
    public void TestFailed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        InstanceName = "cd1",
        ServerRoles = new[] { ServerRole.ContentDelivery },
        Version = new SitecoreVersion(8, 2, 2),
        Configuration = new XmlDocument(),
        IncludeFiles = new Map<ConfigurationFile>(x => x.FilePath)
          {
            {
              new ConfigurationFile("App_Config/Include/ListManagement/Sitecore.ListManagement.config",
                "<configuration><sitecore></sitecore></configuration>")
            },
            {
              new ConfigurationFile("App_Config/Include/ListManagement/Sitecore.ListManagement.Services.config",
                "<configuration><sitecore></sitecore></configuration>")
            },
          },
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
      {
                   { new ReleaseInfo(ServiceClientBuilder.Create().Build().Products["Email Experience Manager"].Versions["3.4.1"]) }
         },
      };


      new SolutionUnitTestContext()
        .AddInstance(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, ErrorMessage(),
        data: GetMessage(
          new List<string>()
          {
            "App_Config/Include/ListManagement/Sitecore.ListManagement.config",
            "App_Config/Include/ListManagement/Sitecore.ListManagement.Services.config"
          })))
        .Done();
    }

    [Fact]
    public void TestPassed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        InstanceName = "cd1",
        ServerRoles = new[] { ServerRole.ContentDelivery },
        Version = new SitecoreVersion(8, 2, 2),
        Configuration = new XmlDocument()
          .Create(
            "/configuration/sitecore/"),
        IncludeFiles = new Map<ConfigurationFile>(x => x.FilePath)
        {
          {
            new ConfigurationFile("App_Config/Include/ListManagement/Sitecore.Helpers.config",
              "<configuration><sitecore></sitecore></configuration>")
          },
          {
            new ConfigurationFile("App_Config/Include/ListManagement/Sitecore.Services.config",
              "<configuration><sitecore></sitecore></configuration>")
          },
        },
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
  }
}
