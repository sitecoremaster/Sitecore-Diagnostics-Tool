namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using JetBrains.Annotations;

  public interface ITestResults : ITestResultsView
  {
    void Add([NotNull] ITestResult testResult);
  }
}