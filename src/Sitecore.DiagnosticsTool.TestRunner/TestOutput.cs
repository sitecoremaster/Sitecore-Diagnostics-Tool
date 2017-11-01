namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public class TestOutput : ITestResult
  {
    public TestOutput(TestResultState state, [NotNull] ShortMessage message, Uri link = null, DetailedMessage data = null)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      State = state;
      Message = message;
      Link = link;
      Detailed = data;
    }

    public TestResultState State { get; }

    public ShortMessage Message { get; }

    public Uri Link { get; }

    public DetailedMessage Detailed { get; }

    /// <summary>
    ///   Indicates if the Sitecore test was completed i.e. it is conclusive.
    ///   return this.State == TestResultState.TestPassed || this.State == TestResultState.TestFailed;
    /// </summary>
    [PublicAPI]
    public bool IsCompleted => State != TestResultState.CannotRun;

    /// <summary>
    ///   Indicates if the Sitecore test was successfully passed.
    ///   return this.State == TestResultState.TestPassed;
    /// </summary>
    [PublicAPI]
    public bool IsWarning => State == TestResultState.Warning;

    /// <summary>
    ///   Indicates if the Sitecore test failed.
    ///   return this.State == TestResultState.TestFailed;
    /// </summary>
    [PublicAPI]
    public bool IsError => State == TestResultState.Error;

    public override string ToString()
    {
      return $"{State}, {Message}";
    }
  }
}