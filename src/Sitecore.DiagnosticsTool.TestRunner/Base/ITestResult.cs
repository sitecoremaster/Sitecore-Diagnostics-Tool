namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  public interface ITestResult : ITestResultData
  {
    TestResultState State { get; }
  }
}