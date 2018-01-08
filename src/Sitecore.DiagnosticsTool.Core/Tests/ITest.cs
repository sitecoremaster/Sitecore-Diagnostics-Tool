namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;

  /// <summary>
  ///   Base interface that is required for every solution-aware test to implement.
  /// </summary>
  public interface ITest : ITestMetadata
  {
    /// <summary>
    ///   Indicate if the test is actual for given Sitecore solution.
    /// </summary>
    bool IsActual([NotNull] ISolutionResourceContext data, [NotNull] ISitecoreVersion sitecoreVersion);

    /// <summary>
    ///   Perform complete test and provide output.
    /// </summary>
    /// <param name="data">An interface to the resources API.</param>
    /// <param name="output">An interface to the output messages API.</param>
    void Process([NotNull] ISolutionResourceContext data, [NotNull] ITestOutputContext output);
  }
}