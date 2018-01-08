namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;

  public interface ISolutionUnitTestContext
  {
    [NotNull]
    ISolutionUnitTestContext AddInstance([NotNull] IDataProvider dataProvider);

    [NotNull]
    ISolutionUnitTestResultsContext Process([NotNull] ITest test);
  }
}