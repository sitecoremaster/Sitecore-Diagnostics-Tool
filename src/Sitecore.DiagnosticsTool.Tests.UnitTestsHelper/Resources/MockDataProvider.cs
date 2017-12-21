namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  public class MockDataProvider : IDataProvider
  {
    private IReadOnlyList<IResource> Resources { get; }

    public MockDataProvider(IReadOnlyList<IResource> resources)
    {
      Assert.ArgumentNotNull(resources, nameof(resources));

      Resources = resources;
    }

    public IEnumerable<IResource> GetResources()
    {
      return Resources;
    }
  }
}