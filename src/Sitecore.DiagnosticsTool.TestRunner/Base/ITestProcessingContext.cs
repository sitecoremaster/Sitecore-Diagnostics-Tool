namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Tests;

  public interface ITestProcessingContext : ITestOutputContext
  {
    [NotNull]
    ITestResults Results { get; }

    void Reset();
  }
}