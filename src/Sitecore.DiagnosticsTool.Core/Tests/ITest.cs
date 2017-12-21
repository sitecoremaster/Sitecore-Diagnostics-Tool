namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;

  /// <inheritdoc />
  /// <summary>
  ///   Base interface that is required for every test to implement.
  /// </summary>
  [UsedImplicitly]
  [Obsolete]
  public interface ILegacyTest : ITestMetadata
  {
    /// <summary>
    ///   Indicate if the test is actual for given Sitecore instance.
    /// </summary>
    bool IsActual([NotNull] IReadOnlyCollection<ServerRole> roles, [NotNull] ISitecoreVersion sitecoreVersion, [NotNull] ITestResourceContext data);

    /// <summary>
    ///   Perform complete test and provide output.
    /// </summary>
    /// <param name="data">An interface to the resources API.</param>
    /// <param name="output">An interface to the output messages API.</param>
    void Process([NotNull] ITestResourceContext data, [NotNull] ITestOutputContext output);
  }
}