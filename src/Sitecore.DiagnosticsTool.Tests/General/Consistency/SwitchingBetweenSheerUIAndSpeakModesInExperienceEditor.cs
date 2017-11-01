namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class SwitchingBetweenSheerUiAndSpeakModesInExperienceEditor : KbTest
  {
    protected const string MvcPageExtendersRootNode = @"/configuration/sitecore/pipelines/mvc.renderPageExtenders";

    protected const string AspnetPageExtendersRootNode = @"/configuration/sitecore/pageextenders";

    protected const string MvcPageExtendersPath = MvcPageExtendersRootNode + "/processor[@type='{0}']";

    protected const string AspnetPageExtendersPath = AspnetPageExtendersRootNode + @"/pageextender[@type='{0}']";

    [NotNull]
    protected static readonly string[] MvcSpeakProcessors =
    {
      "Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders.SpeakRibbon.RenderPageEditorSpeakExtender, Sitecore.Mvc.ExperienceEditor"
    };

    [NotNull]
    protected static readonly string[] MvcSheerUiProcessors =
    {
      "Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders.RenderPageEditorExtender, Sitecore.Mvc.ExperienceEditor",
      "Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders.RenderPreviewExtender, Sitecore.Mvc.ExperienceEditor",
      "Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders.RenderDebugExtender, Sitecore.Mvc.ExperienceEditor"
    };

    [NotNull]
    protected static readonly string[] AspnetSpeakProcessors =
    {
      "Sitecore.ExperienceEditor.Speak.Ribbon.PageExtender.RibbonPageExtender, Sitecore.ExperienceEditor.Speak.Ribbon"
    };

    [NotNull]
    protected static readonly string[] AspnetSheerUiProcessors =
    {
      "Sitecore.Layouts.PageExtenders.PreviewPageExtender, Sitecore.ExperienceEditor",
      "Sitecore.Layouts.PageExtenders.WebEditPageExtender, Sitecore.ExperienceEditor",
      "Sitecore.Layouts.PageExtenders.DebuggerPageExtender, Sitecore.ExperienceEditor"
    };

    public override string KbNumber => "359871";

    public override string KbName { get; } = "Switching between Sheer UI and SPEAK modes in Experience Editor";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var siteConf = data.SitecoreInfo.Configuration;
      var totalResult = ProcessConfiguration(siteConf);
      if (!totalResult)
      {
        Report(output);
      }
    }

    public static bool ProcessConfiguration([NotNull] XmlDocument siteConf)
    {
      Assert.ArgumentNotNull(siteConf, nameof(siteConf));

      bool b;
      return ProcessConfiguration(siteConf, out b);
    }

    public static bool ProcessConfiguration([NotNull] XmlDocument siteConf, out bool isSpeakMode)
    {
      Assert.ArgumentNotNull(siteConf, nameof(siteConf));

      var mvcSpeakProcessorsCount = 0;
      var mvcSheerUiProcesorsCount = 0;
      var aspnetSpeakProcessorsCount = 0;
      var aspnetSheerUiProcessorsCount = 0;

      foreach (var mvcSpeakProcessor in MvcSpeakProcessors)
      {
        var nodeFullPath = string.Format(MvcPageExtendersPath, mvcSpeakProcessor);
        if (siteConf.SelectSingleNode(nodeFullPath) != null)
        {
          mvcSpeakProcessorsCount++;
        }
      }

      foreach (var mvcSheerUiProcesor in MvcSheerUiProcessors)
      {
        var nodeFullPath = string.Format(MvcPageExtendersPath, mvcSheerUiProcesor);
        if (siteConf.SelectSingleNode(nodeFullPath) != null)
        {
          mvcSheerUiProcesorsCount++;
        }
      }

      foreach (var aspnetSpeakProcessor in AspnetSpeakProcessors)
      {
        var nodeFullPath = string.Format(AspnetPageExtendersPath, aspnetSpeakProcessor);
        if (siteConf.SelectSingleNode(nodeFullPath) != null)
        {
          aspnetSpeakProcessorsCount++;
        }
      }

      foreach (var aspnetSheerUiProcessor in AspnetSheerUiProcessors)
      {
        var nodeFullPath = string.Format(AspnetPageExtendersPath, aspnetSheerUiProcessor);
        if (siteConf.SelectSingleNode(nodeFullPath) != null)
        {
          aspnetSheerUiProcessorsCount++;
        }
      }

      var speakOk = MvcSpeakProcessors.Length == mvcSpeakProcessorsCount
        && AspnetSpeakProcessors.Length == aspnetSpeakProcessorsCount
        && mvcSheerUiProcesorsCount == 0
        && aspnetSheerUiProcessorsCount == 0;

      var sheeruiOk = MvcSheerUiProcessors.Length == mvcSheerUiProcesorsCount
        && AspnetSheerUiProcessors.Length == aspnetSheerUiProcessorsCount
        && mvcSpeakProcessorsCount == 0
        && aspnetSpeakProcessorsCount == 0;

      isSpeakMode = speakOk && !sheeruiOk;

      return speakOk != sheeruiOk;
    }
  }
}