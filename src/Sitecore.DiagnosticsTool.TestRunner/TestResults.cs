namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  internal class TestResults : ITestResults
  {
    [NotNull]
    private readonly List<ITestResult> _Results = new List<ITestResult>();

    public IEnumerable<ITestResult> All => _Results;

    public IEnumerable<ITestResult> Warnings
    {
      get
      {
        return _Results.Where(x => x.State == TestResultState.Warning);
      }
    }

    public IEnumerable<ITestResult> Errors
    {
      get
      {
        return _Results.Where(x => x.State == TestResultState.Error);
      }
    }

    public IEnumerable<ITestResult> CannotRun
    {
      get
      {
        return _Results.Where(x => x.State == TestResultState.CannotRun);
      }
    }

    public IList<DetailedMessage> DebugLogs { get; } = new List<DetailedMessage>();

    public void Add(ITestResult testResult)
    {
      Assert.ArgumentNotNull(testResult, nameof(testResult));

      _Results.Add(testResult);
    }
  }
}