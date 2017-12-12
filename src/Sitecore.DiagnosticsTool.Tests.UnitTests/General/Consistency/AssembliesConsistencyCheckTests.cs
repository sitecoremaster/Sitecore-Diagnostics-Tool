namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class AssembliesConsistencyCheckTests : AssembliesConsistencyMatch
  {
    private static readonly SitecoreVersion ProductVersion = new SitecoreVersion(6, 6, 7);

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
          Assemblies = new[] { _Lib1, _Lib2, _Lib3 }
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, VersionInconsistencyMessage, data: new DetailedMessage(new BulletedList(GetVersionInconsistencyMessage(_Lib2, "1.4.0.0")))))
        //.MustReturn(new TestOutput(TestResultState.Warning, SizeInconsistencyMessage, data: new DetailedMessage(new BulletedList(GetSizeInconsistencyMessage(_Lib3, 167736)))))
        .MustReturn(new TestOutput(TestResultState.Error, AssemblyIsMissingMessage, data: new DetailedMessage(new BulletedList(
          GetAssemblyIsMissingMessage("Lucene.Net.dll"),
          GetAssemblyIsMissingMessage("Mvp.Xml.dll"),
          GetAssemblyIsMissingMessage("Newtonsoft.Json.dll"),
          GetAssemblyIsMissingMessage("RadEditor.Net2.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Analytics.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Apps.Loader.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Automation.MarketingAutomation.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Client.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Kernel.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Logging.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Mvc.Analytics.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Mvc.DeviceSimulator.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Mvc.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Mvc.ExperienceEditor.dll"),
          GetAssemblyIsMissingMessage("sitecore.nexus.dll"),
          GetAssemblyIsMissingMessage("Sitecore.NVelocity.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Oracle.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Publishing.WebDeploy.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Security.AntiCsrf.dll"),
          GetAssemblyIsMissingMessage("Sitecore.SegmentBuilder.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Shell.MarketingAutomation.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Update.dll"),
          GetAssemblyIsMissingMessage("Sitecore.Zip.dll"),
          GetAssemblyIsMissingMessage("Stimulsoft.Base.dll"),
          GetAssemblyIsMissingMessage("Stimulsoft.Database.dll"),
          GetAssemblyIsMissingMessage("Stimulsoft.Report.dll"),
          GetAssemblyIsMissingMessage("Stimulsoft.Report.Web.dll"),
          GetAssemblyIsMissingMessage("Stimulsoft.Report.WebDesign.dll"),
          GetAssemblyIsMissingMessage("Telerik.Web.UI.dll"),
          GetAssemblyIsMissingMessage("Telerik.Web.UI.Skins.dll")))))
        .Done();
    }
  }
}