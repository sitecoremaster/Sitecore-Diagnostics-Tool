namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Xml;

  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Objects;
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

  public class ExmSpeakHandlerCheckerTests : ExmSpeakHandlerChecker
  {
    [Fact]
    public void TestPassed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        Configuration = new XmlDocument()
          .Create("/configuration/system.web/httpHandlers"),
        Version = new SitecoreVersion(8, 1, 0),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
          { new ReleaseInfo(new ServiceClient().Products["Email Experience Manager"].Versions["3.2.0"]) }
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
      var configuration = new XmlDocument()
        .Create("/configuration/system.web/httpHandlers/add[@path='sitecore_ecm_speak_request.ashx']");

      var sitecoreConfiguration = new SitecoreInstance
      {
        Configuration = configuration,
        Version = new SitecoreVersion(8, 1, 1),
        InstalledModules = new Map<IReleaseInfo>(x => x.Release.ProductName)
        {
          { new ReleaseInfo(new ServiceClient().Products["Email Experience Manager"].Versions["3.2.0"]) }
        },
      };

      new SolutionUnitTestContext()
        
        .AddInstance(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<httpHandlers>", false)))
        .Done();
    }
  }
}