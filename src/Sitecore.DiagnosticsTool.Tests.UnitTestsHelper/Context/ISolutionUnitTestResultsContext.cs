namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public interface ISolutionUnitTestResultsContext
  {
    [NotNull]
    ISolutionUnitTestResultsContext MustReturn([NotNull] ITestResult testResult, ComparisonMode mode = ComparisonMode.Strict);

    void Done();
  }
}