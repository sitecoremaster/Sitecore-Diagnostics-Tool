namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class PublishEndHtmlClearingHandlers : Test
  {
    [NotNull]
    protected string[] SystemSites { get; } = {"shell", "login", "admin", "service", "modules_shell", "modules_website", "scheduler", "system", "system_layouts", "publisher", "exm"};

    public override string Name => $"The {EventName} HTML cache cleanup configuration";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Performance};

    [NotNull]
    protected virtual string EventName { get; } = "publish:end";

    [NotNull]
    protected string XPath => $"/configuration/sitecore/events/event[@name='{EventName}']/handler";

    [NotNull]
    protected string MessageNoSitesElement => $"The HTML clearing {EventName} event handler does not have <sites hint=\"list\"> child element";

    [NotNull]
    protected string MessageNoHandlerElement => $"The HTML clearing {EventName} event handler was not found";

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.Publishing);
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var configuration = data.SitecoreInfo.Configuration;
      var handler = configuration.SelectElements(XPath).FirstOrDefault();
      if (handler == null)
      {
        output.Warning(MessageNoHandlerElement);
        return;
      }

      if (handler.ChildNodes.Count == 0)
      {
        output.Warning(MessageNoSitesElement);
        return;
      }

      var sitesElement = handler.ChildNodes[0];
      if (sitesElement == null)
      {
        output.Warning(MessageNoSitesElement);
        return;
      }

      // siteNamesInHandler = ["website"] in default Sitecore configuration
      var siteNamesInHandler = sitesElement.ChildNodes.OfType<XmlNode>().Where(x => x != null).Select(x => x.InnerText).ToArray();

      var actualSiteElements = configuration.SelectElements("/configuration/sitecore/sites/site");
      var actualSiteNames = actualSiteElements.Select(x => x.GetAttribute("name"));

      // frontendSiteNames = ["website"] in default Sitecore configuration
      var frontendSiteNames = actualSiteNames.Where(x => !SystemSites.Contains(x));
      foreach (var frontendSiteName in frontendSiteNames)
      {
        if (frontendSiteName != null && !siteNamesInHandler.Contains(frontendSiteName))
        {
          output.Warning(GetErrorMessage(frontendSiteName));
        }
      }
    }

    [NotNull]
    protected string GetErrorMessage([NotNull] string frontendSiteName)
    {
      Assert.ArgumentNotNull(frontendSiteName, nameof(frontendSiteName));

      return $"The front-end site '{frontendSiteName}' is missing in the HTML clearing '{EventName}' event handler";
    }
  }
}