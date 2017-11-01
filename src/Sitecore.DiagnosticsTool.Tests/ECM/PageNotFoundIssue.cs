namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class PageNotFoundIssue : KbTest
  {
    [NotNull]
    protected readonly HashSet<string> DefaultSites = new HashSet<string>
    {
      "shell",
      "login",
      "admin",
      "service",
      "modules_shell",
      "modules_website",
      "scheduler",
      "system",
      "publisher",
      "speak"
    };

    public override string KbNumber => "264285";

    public override string KbName { get; } = "Troubleshooting Page Not Found errors for links in ECM messages";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Ecm};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(output, nameof(output));

      var errorsDetected = false;

      //if (CheckFiles(data, output))
      //{
      //  errorsDetected = true;
      //}

      if (CheckLinkProvider(data, output))
      {
        errorsDetected = true;
      }

      if (CheckPhysicalFolder(data, output))
      {
        errorsDetected = true;
      }

      if (!errorsDetected)
      {
        output.Debug($"For more troubleshooting steps in case of Page Not Found errors in links from ECM messages please refer to the article: {Link}");
      }
    }

    //protected bool CheckFiles([NotNull] ITestResourceContext data, ITestOutputContext output)
    //{
    //  Assert.ArgumentNotNull(data, nameof(data));
    //
    //  // TODO: rework to use data.SitecoreInfo.StaticFiles API
    //  if (!data.SitecoreInfo.Files.Exists(@"sitecore\RedirectUrlPage.aspx"))
    //  {
    //    output.Error(GetMissingRedirectPageMessage(), url: Link);
    //
    //    return false;
    //  }
    //
    //  return true;
    //}

    protected bool CheckLinkProvider([NotNull] ITestResourceContext data, [NotNull] ITestOutputContext output)
    {
      var managerXPath = "/configuration/sitecore/linkManager";
      var configuration = data.SitecoreInfo.Configuration;
      var defaultProviderName = EcmHelper.GetAttributeValue(configuration, managerXPath, "defaultProvider");
      if (string.IsNullOrEmpty(defaultProviderName))
      {
        return true;
      }

      var providerXPath = $"/configuration/sitecore/linkManager/providers/add[@name='{defaultProviderName}']";
      var alwaysIncludeServerUrlValue = EcmHelper.GetAttributeValue(configuration, providerXPath, "alwaysIncludeServerUrl");

      bool alwaysIncludeServerUrl;
      if (!bool.TryParse(alwaysIncludeServerUrlValue, out alwaysIncludeServerUrl) || !alwaysIncludeServerUrl)
      {
        return true;
      }

      Report(output, GetLinkProviderSettingMessage());

      return false;
    }

    protected bool CheckPhysicalFolder([NotNull] ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var sites = data.SitecoreInfo.Configuration.SelectElements("/configuration/sitecore/sites/site");
      foreach (var siteElement in sites)
      {
        if (siteElement == null)
        {
          continue;
        }

        var name = siteElement.GetAttribute("name");
        if (DefaultSites.Contains(name))
        {
          continue;
        }

        var folder = siteElement.GetAttribute("physicalFolder");
        if (string.IsNullOrEmpty(folder))
        {
          continue;
        }

        if (folder == "/")
        {
          continue;
        }

        output.Warning(GetPhysicalFolderSettingMessage(name, folder));

        return false;
      }

      return true;
    }

    [NotNull]
    protected string GetMissingRedirectPageMessage()
    {
      return "The sitecore\\RedirectUrlPage.aspx file doesn't exist - this can cause 404 error when viewing links in email messages.";
    }

    [NotNull]
    protected string GetPhysicalFolderSettingMessage([NotNull] string site, [NotNull] string value)
    {
      Assert.ArgumentNotNull(site, nameof(site));
      Assert.ArgumentNotNull(value, nameof(value));

      return $"The physicalFolder attribute of '{site}' site is set to '{value}' - this can cause Page Not Found errors for links in ECM messages.";
    }

    [NotNull]
    protected string GetLinkProviderSettingMessage()
    {
      return "The \"alwaysIncludeServerUrl\" property of linkProvider configuration is set to true, this can cause Page Not Found errors for links in ECM messages dispatched from a multi-instance environment.";
    }
  }
}