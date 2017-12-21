namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Tests;

  public interface ITestReport
  {
    ITestMetadata Owner { get; }

    [NotNull]
    ITestResultsView Results { get; }

    TimeSpan ExecutionTime { get; set; }
  }
}