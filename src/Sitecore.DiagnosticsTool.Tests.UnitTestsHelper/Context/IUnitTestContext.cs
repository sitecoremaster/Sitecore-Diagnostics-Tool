namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public interface IUnitTestContext
  {
    [NotNull]
    IUnitTestContext AddResource([NotNull] IResource resource);

    [NotNull]
    [Obsolete("Use AddResource(new SitecoreInstance { ServerRoles = ... }) instead.")]
    IUnitTestContext AddResource([NotNull] IServerRolesContext resource);

    [NotNull]
    IUnitTestContext AddResource([NotNull] IDataProvider dataProvider);

    [NotNull]
    IUnitTestResultsContext Process([NotNull] ITest testBase);
  }
}