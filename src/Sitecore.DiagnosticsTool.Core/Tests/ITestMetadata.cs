namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;

  /// <summary>
  ///   The metadata interface for test.
  /// </summary>
  public interface ITestMetadata
  {
    /// <summary>
    ///   Easy to remember and share test name.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    ///   The list of categories the test belongs to.
    /// </summary>
    [NotNull]
    IEnumerable<Category> Categories { get; }
  }
}