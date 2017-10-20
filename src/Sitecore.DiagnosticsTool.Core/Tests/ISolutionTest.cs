namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Objects;

  /// <summary>
  ///   Base interface that is required for every solution-aware test to implement.
  /// </summary>
  public interface ISolutionTest : ITestMetadata
  {
    /// <summary>
    ///   The method indicates if this specific test is actual for Sitecore version of the instance under test.
    /// </summary>
    bool IsActual([NotNull] ISolutionTestResourceContext data, [NotNull] ISitecoreVersion sitecoreVersion);

    /// <summary>
    ///   All the test logic must be placed here. Use data parameter to access the resources API, use output parameter to give
    ///   results.
    /// </summary>
    /// <param name="data">An interface to test resources.</param>
    /// <param name="output">An interface to test output.</param>
    void Process([NotNull] ISolutionTestResourceContext data, [NotNull] ITestOutputContext output);
  }
}