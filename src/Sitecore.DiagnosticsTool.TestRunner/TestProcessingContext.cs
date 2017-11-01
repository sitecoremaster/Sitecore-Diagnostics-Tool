namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public sealed class TestProcessingContext : ITestProcessingContext, IDisposable
  {
    public void Dispose()
    {
      // this was added to avoid Resharper false suggestions
    }

    public ITestResults Results { get; private set; } = new TestResults();

    public void Reset()
    {
      Results = new TestResults();
    }

    public void Error(ShortMessage message, Uri url, DetailedMessage detailed)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      AddResult(TestResultState.Error, url, message, detailed);
    }

    public void Warning(ShortMessage message, Uri url, DetailedMessage detailed)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      AddResult(TestResultState.Warning, url, message, detailed);
    }

    public void CannotRun(ShortMessage message, Uri url, DetailedMessage detailed)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      AddResult(TestResultState.CannotRun, url, message, detailed);
    }

    public void Debug(DetailedMessage message)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      Results.DebugLogs.Add(message);
    }

    public void Debug(Exception ex, DetailedMessage message)
    {
      Assert.ArgumentNotNull(ex, nameof(ex));
      Assert.ArgumentNotNull(message, nameof(message));

      Debug(new DetailedMessage(message.Items.Concat(new[] {new Text(". Exception:"), new CodeBlock(ex.PrintException())})));
    }

    private void AddResult(TestResultState state, [CanBeNull] Uri link, [NotNull] ShortMessage message, [CanBeNull] DetailedMessage detailed)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      if (Results.All.Any(x => x.State == state && x.Message.ToString() == message.ToString()))
      {
        // avoid duplicate messages
        return;
      }

      Log.Info($"Test output: {state}, {message}{(link != null ? ", " + link.AbsoluteUri : null)}");
      Results.Add(new TestOutput(state, message, link, detailed));
    }
  }
}