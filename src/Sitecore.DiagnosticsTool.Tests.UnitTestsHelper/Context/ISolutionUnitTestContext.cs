namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public interface ISolutionUnitTestContext
  {
    [NotNull]
    ISolutionUnitTestContext AddInstance([NotNull] IDataProvider dataProvider);

    [NotNull]
    ISolutionUnitTestResultsContext Process([NotNull] ISolutionTest test);
  }
}