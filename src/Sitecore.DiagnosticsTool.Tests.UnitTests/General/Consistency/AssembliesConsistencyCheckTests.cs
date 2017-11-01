namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class AssembliesConsistencyCheckTests : AssembliesConsistencyMatch
  {
    private static readonly SitecoreVersion ProductVersion = new SitecoreVersion(6, 6, 7, 131211);

    private readonly AssemblyFile _Lib1 = new AssemblyFile("ComponentArt.Web.UI.dll", "2010.1.2637.35", "2010.1.2637.35", 2773680);

    private readonly AssemblyFile _Lib2 = new AssemblyFile("HtmlAgilityPack.dll", "1.4.6.0", "1.4.6.0", 143160);

    private readonly AssemblyFile _Lib3 = new AssemblyFile("ITHit.WebDAV.Server.dll", "2.1.1.108", "2.1.1.108", 99);

    [Fact]
    public void AssembliesConsistencyCheckTest()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Assemblies = new[] {_Lib1, _Lib2, _Lib3}
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetVersionInconsistencyMessage(_Lib2, "1.4.0.0")))
        .MustReturn(new TestOutput(TestResultState.Warning, GetSizeInconsistencyMessage(_Lib3, 167736)))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Lucene.Net.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Mvp.Xml.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Newtonsoft.Json.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("RadEditor.Net2.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Analytics.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Apps.Loader.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Automation.MarketingAutomation.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Client.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Kernel.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Logging.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Mvc.Analytics.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Mvc.DeviceSimulator.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Mvc.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Mvc.ExperienceEditor.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("sitecore.nexus.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.NVelocity.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Oracle.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Publishing.WebDeploy.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Security.AntiCsrf.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.SegmentBuilder.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Shell.MarketingAutomation.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Update.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Sitecore.Zip.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Stimulsoft.Base.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Stimulsoft.Database.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Stimulsoft.Report.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Stimulsoft.Report.Web.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Stimulsoft.Report.WebDesign.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Telerik.Web.UI.dll")))
        .MustReturn(new TestOutput(TestResultState.Error, GetAssemblyIsMissingMessage("Telerik.Web.UI.Skins.dll")))
        .Done();
    }
  }
}