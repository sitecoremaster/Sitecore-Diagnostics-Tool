namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class MockDataProvider : IDataProvider
  {
    private IReadOnlyList<IResource> Resources { get; }

    public MockDataProvider(IReadOnlyList<IResource> resources)
    {
      Assert.ArgumentNotNull(resources, nameof(resources));

      var info = resources.OfType<ISitecoreInformationContext>().FirstOrDefault();
      if (info != null)
      {
        info.InstanceName = InstanceName;
      }

      Resources = resources;
    }

    public string InstanceName { get; set; } = "";

    public IEnumerable<IResource> GetResources()
    {
      return Resources;
    }
  }
}