namespace Sitecore.DiagnosticsTool.UnitTests.Common.DataProviders
{
  using System.Collections.Generic;
  using System.Xml;

  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class EmptyDataProvider : IDataProvider
  {
    private IServiceClient ServiceClient { get; }

    private SitecoreVersion Version { get; }

    public EmptyDataProvider(SitecoreVersion version, IServiceClient client = null)
    {
      Version = version;
      ServiceClient = client ?? new ServiceClient();
    }

    public IEnumerable<IResource> GetResources()
    {
      yield return new ServerRolesContext(ServerRole.ContentManagement | ServerRole.ContentDelivery);

      if (Version != null)
      {
        yield return new SitecoreInformationContext(ServiceClient)
        {
          SitecoreVersionXmlFile = new XmlDocument()
            .Parse($"<information>" +
              $"<version>" +
              $"<major>{Version.Major}</major>" +
              $"<minor>{Version.Minor}</minor>" +
              $"<build>{0}</build>" +
              $"<revision>{ServiceClient.Products["Sitecore CMS"].Versions[Version.MajorMinorUpdate].Revision}</revision>" +
              $"</version><title>Sitecore.NET</title><company>Sitecore Corporation</company><copyright>© Sitecore. All rights reserved.</copyright></information>")
        };
      }
    }
  }
}