// <summary>
//   http://issues.pssbuild1dk1.dk.sitecore.net/issue/SDT-78
// </summary>

namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class WebsiteFolderReusage : Test
  {
    public override string Name { get; } = "Website folder reusage by other IIS sites";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var site = data.WebServer.CurrentSite;
      var websitePath = site.WebRoot.FullName;
      var allSites = data.WebServer.Sites;

      foreach (var otherSite in allSites)
      {
        if (otherSite == null || site.Id == otherSite.Id)
        {
          continue;
        }

        var anotherWebsitePath = otherSite.WebRoot.FullName;
        if (anotherWebsitePath.Equals(websitePath, StringComparison.OrdinalIgnoreCase))
        {
          var message = GetErrorMessage(site, otherSite, anotherWebsitePath);

          output.Error(message);
        }
      }
    }

    [NotNull]
    protected static string GetErrorMessage([NotNull] ISite site, [NotNull] ISite otherSite, [NotNull] string anotherWebsitePath)
    {
      Assert.ArgumentNotNull(site, nameof(site));
      Assert.ArgumentNotNull(otherSite, nameof(otherSite));
      Assert.ArgumentNotNull(anotherWebsitePath, nameof(anotherWebsitePath));

      return $"Current IIS site (\"{site.Name}\") points to the same website folder (\"{anotherWebsitePath}\") as another \"{otherSite.Name}\" site, which is not supported. Please check Installation Guide.";
    }
  }
}