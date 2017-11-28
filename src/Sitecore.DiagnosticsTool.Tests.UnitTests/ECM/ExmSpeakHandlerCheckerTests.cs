namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
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
        Assemblies = new[]
        {
          new AssemblyFile("Sitecore.EmailCampaign.dll", productVersion: "3.2 rev. 160127")
        }
      };
      UnitTestContext
        .Create(this)
        .AddResource(sitecoreConfiguration)
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
        Assemblies = new[]
        {
          new AssemblyFile("Sitecore.EmailCampaign.dll", productVersion: "3.2 rev. 160127")
        }
      };

      UnitTestContext
        .Create(this)
        .AddResource(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<httpHandlers>", false)))
        .Done();
    }
  }
}