namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Output;

  public interface ITestResultsView
  {
    [NotNull]
    IEnumerable<ITestResult> All { get; }

    [NotNull]
    IEnumerable<ITestResult> Warnings { get; }

    [NotNull]
    IEnumerable<ITestResult> Errors { get; }

    [NotNull]
    IEnumerable<ITestResult> CannotRun { get; }

    [NotNull]
    IList<DetailedMessage> DebugLogs { get; }
  }
}