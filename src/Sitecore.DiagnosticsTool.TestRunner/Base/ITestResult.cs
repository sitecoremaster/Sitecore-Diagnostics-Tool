namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Output;

  public interface ITestResult
  {
    TestResultState State { get; }

    [NotNull]
    ShortMessage Message { get; }

    [CanBeNull]
    Uri Link { get; }

    [CanBeNull]
    DetailedMessage Detailed { get; }
  }
}