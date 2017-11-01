namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;

  /// <summary>
  ///   Base interface that is required for every test to implement.
  /// </summary>
  [UsedImplicitly]
  public interface ITest : ITestMetadata
  {
    /// <summary>
    ///   The method indicates if this specific test is actual for Sitecore version of the instance under test.
    /// </summary>
    bool IsActual([NotNull] IReadOnlyCollection<ServerRole> roles, [NotNull] ISitecoreVersion sitecoreVersion, [NotNull] ITestResourceContext data);

    /// <summary>
    ///   All the test logic must be placed here. Use data parameter to access the resources API, use output parameter to give
    ///   results.
    /// </summary>
    /// <param name="data">An interface to test resources.</param>
    /// <param name="output">An interface to test output.</param>
    void Process([NotNull] ITestResourceContext data, [NotNull] ITestOutputContext output);
  }
}