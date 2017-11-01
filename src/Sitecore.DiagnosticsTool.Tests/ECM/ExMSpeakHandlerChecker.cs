namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class ExmSpeakHandlerChecker : Test
  {
    private const string ecmSpeakHandlerPath = "sitecore_ecm_speak_request.ashx";

    public override string Name { get; } = "SPEAK handlers are present in web.config";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Ecm};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 7;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var ecmVersion = EcmHelper.GetEcmVersion(data);
      if (ecmVersion == null)
      {
        return;
      }

      var majorMinor = ecmVersion.MajorMinorInt;
      if (majorMinor >= 31)
      {
        var httpHandlersNode = data.SitecoreInfo.Configuration.SelectSingleNode($"/configuration/system.web/httpHandlers/add[@path='{ecmSpeakHandlerPath}']");
        if (httpHandlersNode != null)
        {
          output.Warning(GetErrorMessage("<httpHandlers>", false));
        }
        var handlersNode = data.SitecoreInfo.Configuration.SelectSingleNode($"/configuration/system.webSerber/handlers/add[@path='{ecmSpeakHandlerPath}']");
        if (handlersNode != null)
        {
          output.Warning(GetErrorMessage("<handlers>", false));
        }
      }
      if (majorMinor == 22 || majorMinor == 30)
      {
        var httpHandlersNode = data.SitecoreInfo.Configuration.SelectSingleNode($"/configuration/system.web/httpHandlers/add[@path='{ecmSpeakHandlerPath}']");
        if (httpHandlersNode == null)
        {
          output.Warning(GetErrorMessage("<httpHandlers>", true));
        }
        var handlersNode = data.SitecoreInfo.Configuration.SelectSingleNode($"/configuration/system.webSerber/handlers/add[@path='{ecmSpeakHandlerPath}']");
        if (handlersNode == null)
        {
          output.Warning(GetErrorMessage("<handlers>", true));
        }
      }
    }

    public string GetErrorMessage(string section, bool shouldBePresent)
    {
      string result;
      Assert.ArgumentNotNull(section, nameof(section));
      Assert.ArgumentNotNull(shouldBePresent, nameof(shouldBePresent));

      if (!shouldBePresent)
      {
        result = $"The sitecore_ecm_speak_handler.ashx handler should be removed from the '{section}' section of the web.config file.";
      }
      else
      {
        result = $"The sitecore_ecm_speak_handler.ashx handler is missed in the '{section}' section of the web.config file.";
      }

      return result;
    }
  }
}