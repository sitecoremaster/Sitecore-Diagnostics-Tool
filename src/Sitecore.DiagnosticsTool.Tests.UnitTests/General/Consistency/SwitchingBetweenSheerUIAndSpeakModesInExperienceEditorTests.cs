namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class SwitchingBetweenSheerUiAndSpeakModesInExperienceEditorTests : SwitchingBetweenSheerUiAndSpeakModesInExperienceEditor
  {
    private const string ProcessorFormat = "processor[@type='{0}']";

    private const string ExtenderFormat = "pageextender[@type='{0}']";

    [Fact]
    public void CheckIsSheerUiRibbon()
    {
      UnitTestContext
        .Create(this)
        .AddResource(GetCorrectSheeruiConfig())
        .Process(this)
        .Done();
    }

    [Fact]
    public void CheckIsSpeakRibbon()
    {
      UnitTestContext
        .Create(this)
        .AddResource(GetCorrectSpeakConfig())
        .Process(this)
        .Done();
    }

    [Fact]
    public void CheckMixedModeRibbon()
    {
      UnitTestContext
        .Create(this)
        .AddResource(GetMixedConfig())
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();
    }

    [Fact]
    public void CheckInvalidVersion()
    {
      UnitTestContext
        .Create(this)
        .AddResource(GetInvalidVersion())
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"))
        .Done();
    }

    public static SitecoreInstance GetCorrectSheeruiConfig()
    {
      var doc = new XmlDocument()
        .Create(AspnetPageExtendersRootNode)
        .Add(@"/configuration/sitecore", "pipelines")
        .Add(@"/configuration/sitecore/pipelines", "mvc.renderPageExtenders");
      foreach (var aspnetSheerUiProcessor in AspnetSheerUiProcessors)
      {
        var processor = string.Format(ExtenderFormat, aspnetSheerUiProcessor);
        doc.Add(AspnetPageExtendersRootNode, processor);
      }

      foreach (var mvcSheerUiProcessor in MvcSheerUiProcessors)
      {
        var processor = string.Format(ProcessorFormat, mvcSheerUiProcessor);
        doc.Add(MvcPageExtendersRootNode, processor);
      }

      return new SitecoreInstance
      {
        Configuration = doc,
        Version = new SitecoreVersion(8, 0, 0, 141212)
      };
    }

    public static SitecoreInstance GetCorrectSpeakConfig()
    {
      var doc = new XmlDocument()
        .Create(AspnetPageExtendersRootNode)
        .Add(@"/configuration/sitecore", "pipelines")
        .Add(@"/configuration/sitecore/pipelines", "mvc.renderPageExtenders");
      foreach (var aspnetSpeakProcessor in AspnetSpeakProcessors)
      {
        var processor = string.Format(ExtenderFormat, aspnetSpeakProcessor);
        doc.Add(AspnetPageExtendersRootNode, processor);
      }

      foreach (var mvcSpeakProcessor in MvcSpeakProcessors)
      {
        var processor = string.Format(ProcessorFormat, mvcSpeakProcessor);
        doc.Add(MvcPageExtendersRootNode, processor);
      }

      return new SitecoreInstance
      {
        Configuration = doc,
        Version = new SitecoreVersion(8, 0, 0, 141212)
      };
    }

    public static SitecoreInstance GetMixedConfig()
    {
      var mixedConfig = GetCorrectSpeakConfig();
      var processor = string.Format(ExtenderFormat, AspnetSheerUiProcessors[0]);
      mixedConfig.Configuration.Add(AspnetPageExtendersRootNode, processor);
      return mixedConfig;
    }

    public static SitecoreInstance GetInvalidVersion()
    {
      return new SitecoreInstance
      {
        Version = new SitecoreVersion(7, 5, 0, 141003)
      };
    }
  }
}