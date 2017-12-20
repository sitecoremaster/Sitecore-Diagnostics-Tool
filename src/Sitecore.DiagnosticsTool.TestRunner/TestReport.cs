namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public class TestReport : ITestReport
  {
    [NotNull]
    private ITestResults _Results { get; }

    public TestReport([NotNull] ITestMetadata test, [NotNull] ITestResults results)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(results, nameof(results));

      Owner = test;
      _Results = results;
    }

    public ITestMetadata Owner { get; }

    public ITestResultsView Results => _Results;

    public TimeSpan ExecutionTime { get; set; }
  }
}