namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class UnitTestContext : IUnitTestContext
  {
    [ItemNotNull]
    [NotNull]
    private readonly List<IResource> _Resources = new List<IResource>();

    [ItemNotNull]
    [NotNull]
    private readonly List<IDataProvider> _DataProviders = new List<IDataProvider>();

    private UnitTestContext()
    {
    }

    public IUnitTestContext AddResource(IResource resource)
    {
      Assert.ArgumentNotNull(resource, nameof(resource));

      _Resources.Add(resource);

      return this;
    }

    public IUnitTestContext AddResource(IServerRolesContext resource)
    {
      throw new NotImplementedException();
    }

    public IUnitTestContext AddResource(IDataProvider dataProvider)
    {
      Assert.ArgumentNotNull(dataProvider, nameof(dataProvider));

      _DataProviders.Add(dataProvider);

      return this;
    }

    public IUnitTestResultsContext Process(ILegacyTest test)
    {
      Assert.ArgumentNotNull(test, nameof(test));

      var resources = _Resources;
      foreach (var dataProvider in _DataProviders)
      {
        resources.AddRange(dataProvider.GetResources());
      }

      if (!resources.OfType<IServerRolesContext>().Any())
      {
        resources.Add(new ServerRolesContext(ServerRole.ContentManagement | ServerRole.ContentDelivery | ServerRole.ContentIndexing | ServerRole.Processing | ServerRole.Reporting));
      }

      return new UnitTestResultsContext(test, resources);
    }

    [NotNull]
    public static IUnitTestContext Create([NotNull] ILegacyTest test)
    {
      Assert.ArgumentNotNull(test, nameof(test));

      return new UnitTestContext();
    }
  }
}